using Marketing_system.BL.Contracts.DTO;
using Marketing_system.BL.Contracts.IService;
using Marketing_system.DA.Contracts;
using Marketing_system.DA.Contracts.Model;
using Marketing_system.DA.Contracts.Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;



namespace Marketing_system.BL.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthenticationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthenticationTokensDto?> Login(string email, string password)
        {
            var user = await _unitOfWork.GetUserRepository().GetByEmailAsync(email);
            if (user != null)
            {
                var hashedPassword = _unitOfWork.GetUserRepository().GetPasswordByEmail(email);
                if (_unitOfWork.GetPasswordHasher().VerifyPassword(password, hashedPassword))
                {
                    var tokens = await _unitOfWork.GetTokenGeneratorRepository().GenerateTokens(user);
                    user.RefreshToken = tokens.RefreshToken;
                    _unitOfWork.GetUserRepository().Update(user);
                    await _unitOfWork.Save();
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
                await _unitOfWork.GetUserRepository().Add(new User(userDto.Email, password, userDto.Firstname, userDto.Lastname, userDto.Address, userDto.City, userDto.Country, userDto.Phone, (UserRole)userDto.Role, ClientType.Individual, (PackageType)userDto.PackageType, AccountStatus.Requested));
            }
            else
            {
                await _unitOfWork.GetUserRepository().Add(new User(userDto.Email, password, userDto.CompanyName, userDto.TaxId, userDto.Address, userDto.City, userDto.Country, userDto.Phone, (UserRole)userDto.Role, (ClientType)userDto.ClientType, (PackageType)userDto.PackageType));
            }

            await _unitOfWork.GetRegistrationRequestRepository().Add(new RegistrationRequest(userDto.Firstname, userDto.Lastname, userDto.Email, DateTime.Now.ToUniversalTime(), (PackageType)userDto.PackageType));
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
            if (user == null || user.RefreshToken != refreshToken)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ValidateAccessToken(string accessToken)
        {
            return await _unitOfWork.GetTokenGeneratorRepository().ValidateAccessToken(accessToken);
        }


        // TODO: Add to IAuthenticationService
        // Returns `true` if email is successfully sent.
        public async Task<bool?> SendPasswordlessLogin(string email)
        {
            var user = await _unitOfWork.GetUserRepository().GetByEmailAsync(email);
            if (user is null)
            {
                return null;
            }
            if (user.PackageType == PackageType.Basic)
            {
                return null;
            }

            var token = GeneratePasswordlessToken();
            // TODO: Update if Authentication controller APIs are changed!
            var link = $"https://localhost:7198/api/users/authenticatePasswordlessLogin?token={token}";

            await _unitOfWork.GetPasswordlessTokenRepository().Add(
                new PasswordlessToken()
                {
                    Token = token,
                    ExpirationDate = DateTime.UtcNow.AddMinutes(10),
                    IsUsed = false,
                    UserId = user.Id
                });
            await _unitOfWork.Save();

            return await _unitOfWork.GetEmailHandler().SendPasswordlessLink(email, link);
        }

        public AuthenticationTokensDto? AuthenticatePasswordlessToken(string email, string token)
        {
            // TODO: Check if passwordless token is valid, generate JWT tokens and return them


            throw new NotImplementedException();
        }

        private static string GeneratePasswordlessToken()
        {
            var secretKey = "temp_secret_key"; // TODO: Move this to appsettings.json at least
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var payload = $"{timestamp}.{secretKey}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var tokenBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var token = Convert.ToBase64String(tokenBytes);

            return token;
        }
    }
}
