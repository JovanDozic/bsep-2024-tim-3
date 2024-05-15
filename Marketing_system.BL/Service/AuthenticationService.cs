using Marketing_system.BL.Contracts.DTO;
using Marketing_system.BL.Contracts.IService;
using Marketing_system.DA.Contracts;
using Marketing_system.DA.Contracts.IRepository;
using Marketing_system.DA.Contracts.Model;
using Marketing_system.DA.Contracts.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;



namespace Marketing_system.BL.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HMACConfig _hmacConfig;
        private readonly SMTPConfig _smtpConfig;
        private readonly IEmailHandler _emailHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(IUnitOfWork unitOfWork, IOptions<HMACConfig> hmacConfig, IOptions<SMTPConfig> smtpConfig, IEmailHandler emailHandler, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _hmacConfig = hmacConfig.Value;
            _smtpConfig = smtpConfig.Value;
            _emailHandler = emailHandler;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TokensDto?> Login(string email, string password)
        {
            var user = await _unitOfWork.GetUserRepository().GetByEmailAsync(email);
            if (user != null)
            {
                if (user.AccountStatus != AccountStatus.Active) return null;

                var hashedPassword = _unitOfWork.GetUserRepository().GetPasswordByEmail(email);
                if (_unitOfWork.GetPasswordHasher().VerifyPassword(password, hashedPassword))
                {
                    var tokens = await _unitOfWork.GetTokenGeneratorRepository().GenerateTokens(user);
                    SetRefreshTokenCookie(tokens.RefreshToken);
                    return tokens;
                }
            }
            return null;
        }

        public async Task<bool> RegisterUser(UserDto userDto)
        {
            var userdb = await _unitOfWork.GetUserRepository().GetByEmailAsync(userDto.Email);
            if (userdb != null)
            {
                return false;
            }

            var password = _unitOfWork.GetPasswordHasher().HashPassword(userDto.Password);

            if ((ClientType)userDto.ClientType == ClientType.Individual)
            {
                await _unitOfWork.GetUserRepository().Add(new User(userDto.Email, password, userDto.Firstname, userDto.Lastname, userDto.Address, userDto.City, userDto.Country, userDto.Phone, (UserRole)userDto.Role, ClientType.Individual, (PackageType)userDto.PackageType, AccountStatus.Requested, null, null));
            }
            else
            {
                await _unitOfWork.GetUserRepository().Add(new User(userDto.Email, password, null, null, userDto.Address, userDto.City, userDto.Country, userDto.Phone, (UserRole)userDto.Role, (ClientType)userDto.ClientType, (PackageType)userDto.PackageType, AccountStatus.Requested, userDto.CompanyName, userDto.TaxId));
            }

            await _unitOfWork.GetRegistrationRequestRepository().Add(new RegistrationRequest(userDto.Firstname, userDto.Lastname, userDto.Email, DateTime.Now.ToUniversalTime(), (PackageType)userDto.PackageType));
            await _unitOfWork.Save();
            return true;
        }

        public async Task<bool> RegisterAdminOrEmployee(UserDto userDto)
        {
            var userdb = await _unitOfWork.GetUserRepository().GetByEmailAsync(userDto.Email);
            if (userdb != null)
            {
                return false;
            }

            var password = _unitOfWork.GetPasswordHasher().HashPassword(userDto.Password);

            await _unitOfWork.GetUserRepository().Add(new User(userDto.Email, password, userDto.Firstname, userDto.Lastname, userDto.Address, userDto.City, userDto.Country, userDto.Phone, (UserRole)userDto.Role, ClientType.Individual, (PackageType)userDto.PackageType, AccountStatus.Active, null, null));

            await _unitOfWork.Save();
            return true;
        }
        public async Task<string> UpdateAccessToken(int userId)
        {
            var user = await _unitOfWork.GetUserRepository().GetByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var accessClaims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("id", user.Id.ToString()),
                new(ClaimTypes.Role, user.GetPrimaryRoleName())
            };
            var newRefreshToken = _unitOfWork.GetTokenGeneratorRepository().CreateAccessToken(accessClaims, 15);

            await _unitOfWork.Save();

            return newRefreshToken;
        }

        public async Task<bool> ValidateRefreshToken(int userId, string refreshToken)
        {
            var user = await _unitOfWork.GetUserRepository().GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ValidateAccessToken(string accessToken)
        {
            return await _unitOfWork.GetTokenGeneratorRepository().ValidateAccessToken(accessToken);
        }


        public async Task<bool?> SendPasswordlessLogin(string email)
        {
            var user = await _unitOfWork.GetUserRepository().GetByEmailAsync(email);
            if (user is null)
            {
                return null;
            }
            if (user.PackageType == PackageType.Basic || user.AccountStatus != AccountStatus.Active)
            {
                return null;
            }

            var token = GeneratePasswordlessToken(email);
            var link = $"http://localhost:4200/authenticate-passwordless?token={token}";

            await _unitOfWork.GetPasswordlessTokenRepository().Add(
                new PasswordlessToken()
                {
                    Token = token,
                    ExpirationDate = DateTime.UtcNow.AddMinutes(10),
                    IsUsed = false,
                    UserId = user.Id
                });
            await _unitOfWork.Save();

            return await _emailHandler.SendPasswordlessLink(email, link);
        }

        public async Task<TokensDto?> AuthenticatePasswordlessTokenAsync(string token)
        {
            var passwordlessToken = await _unitOfWork.GetPasswordlessTokenRepository().GetByTokenAsync(token);
            if (passwordlessToken == null ||
                passwordlessToken.IsUsed ||
                passwordlessToken.ExpirationDate < DateTime.UtcNow)
            {
                return null;
            }

            passwordlessToken.IsUsed = true;
            _unitOfWork.GetPasswordlessTokenRepository().Update(passwordlessToken);
            await _unitOfWork.Save();

            var user = await _unitOfWork.GetUserRepository().GetByIdAsync(passwordlessToken.UserId);
            if (user is null)
            {
                return null;
            }

            var tokens = await _unitOfWork.GetTokenGeneratorRepository().GenerateTokens(user);
            return tokens;
        }

        private string GeneratePasswordlessToken(string email)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var payload = $"{email}:{timestamp}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_hmacConfig.Secret));
            var tokenBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var token = Convert.ToBase64String(tokenBytes);

            return token;
        }

        private bool ValidatePasswordlessToken(string token)
        {
            var parts = token.Split(':');
            if (parts.Length != 2)
            {
                return false;
            }

            var email = parts[0];
            var timestamp = parts[1];

            var payload = $"{email}:{timestamp}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_hmacConfig.Secret));
            var computedToken = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));
            return token == computedToken;
        }
        public async Task<UserDto> GetUserById(int userId)
        {
            var user = await _unitOfWork.GetUserRepository().GetByIdAsync(userId);
            if (user == null)
            {
                return null; // User not found
            }

            // Map User entity to UserDto
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Address = user.Address,
                City = user.City,
                Country = user.Country,
                Phone = user.Phone,
                CompanyName = user.CompanyName,
                TaxId = user.TaxId,
                PackageType = (int)user.PackageType, // Convert PackageType enum to int
                ClientType = (int)user.ClientType, // Convert ClientType enum to int
                Role = (int)user.Role // Convert UserRole enum to int
            };

            return userDto;
        }
        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            var users = await _unitOfWork.GetUserRepository().GetAll();

            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Address = user.Address,
                City = user.City,
                Country = user.Country,
                Phone = user.Phone,
                CompanyName = user.CompanyName,
                TaxId = user.TaxId,
                PackageType = (int)user.PackageType,
                ClientType = (int)user.ClientType,
                Role = (int)user.Role
            });
            return userDtos;
        }

        public async Task<bool> UpdateUser(UserDto user)
        {
            var userToUpdate = await _unitOfWork.GetUserRepository().GetByIdAsync(user.Id);
            if (userToUpdate == null)
            {
                return false; // User not found
            }

            // Update user properties
            userToUpdate.Password = _unitOfWork.GetPasswordHasher().HashPassword(user.Password);
            userToUpdate.Firstname = user.Firstname;
            userToUpdate.Lastname = user.Lastname;
            userToUpdate.Address = user.Address;
            userToUpdate.City = user.City;
            userToUpdate.Country = user.Country;
            userToUpdate.Phone = user.Phone;
            userToUpdate.CompanyName = user.CompanyName;
            userToUpdate.TaxId = user.TaxId;
            userToUpdate.Role = (UserRole)user.Role;
            userToUpdate.ClientType = (ClientType)user.ClientType;
            userToUpdate.PackageType = (PackageType)user.PackageType;

            _unitOfWork.GetUserRepository().Update(userToUpdate);
            await _unitOfWork.Save();

            return true;
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var response = _httpContextAccessor.HttpContext.Response;

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(1)
            };

            response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

    }
}
