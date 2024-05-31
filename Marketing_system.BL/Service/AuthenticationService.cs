using Google.Authenticator;
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
        private readonly ITempTokenManagerService _tempTokens;
        private readonly IEncryptionService _encryptionService;


        public AuthenticationService(IUnitOfWork unitOfWork, IOptions<HMACConfig> hmacConfig, IOptions<SMTPConfig> smtpConfig, IEmailHandler emailHandler, IHttpContextAccessor httpContextAccessor, IEncryptionService encryptionService, ITempTokenManagerService tempTokens)
        {
            _unitOfWork = unitOfWork;
            _hmacConfig = hmacConfig.Value;
            _smtpConfig = smtpConfig.Value;
            _emailHandler = emailHandler;
            _httpContextAccessor = httpContextAccessor;
            _encryptionService = encryptionService;
            _tempTokens = tempTokens;
        }

        public async Task<TokensDto?> Login(string email, string password)
        {
            var user = await _unitOfWork.GetUserRepository().GetByEmailAsync(email);

            if (user is null ||
                user.AccountStatus != AccountStatus.Active ||
                !_unitOfWork.GetPasswordHasher().VerifyPassword(password, user.Password))
            {
                return null;
            }

            user = DecryptUser(user);

            if (!user.IsTwoFactorEnabled)
            {
                var tokens = await _unitOfWork.GetTokenGeneratorRepository().GenerateTokens(user);
                SetRefreshTokenCookie(tokens.RefreshToken);
                return tokens;
            }

            

            var response = new TokensDto
            {
                Id = user.Id,
                AccessToken = string.Empty,
                RefreshToken = string.Empty,
                IsTwoFactorEnabled = true,
                TempToken = _unitOfWork.GetTokenGeneratorRepository().GenerateTempToken(user.Email)
            };

            _tempTokens.AddToken(response.TempToken, user.Email);

            return response;
        }

        public User DecryptUser(User user)
        {
            user.Address = _encryptionService.Decrypt(user.Address);
            user.City = _encryptionService.Decrypt(user.City);
            user.Country = _encryptionService.Decrypt(user.Country);
            user.Firstname = _encryptionService.Decrypt(user.Firstname);
            user.Lastname = _encryptionService.Decrypt(user.Lastname);
            user.Phone = _encryptionService.Decrypt(user.Phone);

            return user;
        }

        public async Task<TokensDto?> LoginVerify2fa(Verify2faDto verifyDto)
        {
            if (string.IsNullOrEmpty(verifyDto.TempToken) || string.IsNullOrEmpty(verifyDto.Code))
            {
                return null;
            }

            if (!_tempTokens.TryGetEmail(verifyDto.TempToken, out string? email) || email is null)
            {
                return null;
            }

            var user = await _unitOfWork.GetUserRepository().GetByEmailAsync(email);
            if (user is null) return null;

            TwoFactorAuthenticator tfa = new();
            if (tfa.ValidateTwoFactorPIN(user.TwoFactorSecret, verifyDto.Code, true))
            {
                var tokens = await _unitOfWork.GetTokenGeneratorRepository().GenerateTokens(user);
                SetRefreshTokenCookie(tokens.RefreshToken);
                _tempTokens.RemoveTokenByEmail(email);
                return tokens;
            }

            return null;
        }





        public async Task<RegistrationResponseDto> RegisterUser(UserDto userDto)
        {
            // Check if there is a user with the same email in the database.
            var userDb = await _unitOfWork.GetUserRepository().GetByEmailAsync(userDto.Email);
            if (userDb != null)
            {
                return new() { IsSuccess = false, Message = "User with provided email already exists." };
            }

            // Check if there is a pending registration request for the same email.
            var registrationRequests = _unitOfWork.GetRegistrationRequestRepository().GetAllByEmailAsync(userDto.Email);
            if (registrationRequests.Any(req => req.Status == DA.Contracts.Model.RegistrationRequestStatus.Pending))
            {
                // If there is a pending registration request for the same email, return false
                return new() { IsSuccess = false, Message = "Account with provided email already has a pending registration request." };
            }
            else if (registrationRequests.Where(req => req.Status == DA.Contracts.Model.RegistrationRequestStatus.Rejected)
                                         .Any(req => req.RegistrationDate >= DateTime.UtcNow.AddHours(-24)))
            {
                // If there is a rejected registration request for the same email in the last 24 hours, return false
                return new() { IsSuccess = false, Message = "Account with provided email was recently rejected. Please try again soon." };
            }

            var user = new User // Mili zasto nismo mapper koristili? Ne znam, promijeni
            {
                Email = userDto.Email,
                Password = _unitOfWork.GetPasswordHasher().HashPassword(userDto.Password),
                Firstname = ((ClientType)userDto.ClientType == ClientType.Legal_entity) ? null : _encryptionService.Encrypt(userDto.Firstname),
                Lastname = ((ClientType)userDto.ClientType == ClientType.Legal_entity) ? null : _encryptionService.Encrypt(userDto.Lastname),
                Address = _encryptionService.Encrypt(userDto.Address),
                City = _encryptionService.Encrypt(userDto.City),
                Country = _encryptionService.Encrypt(userDto.Country),
                Phone = _encryptionService.Encrypt(userDto.Phone),
                Role = (UserRole)userDto.Role,
                ClientType = (ClientType)userDto.ClientType,
                PackageType = (PackageType)userDto.PackageType,
                AccountStatus = AccountStatus.Requested,
                CompanyName = ((ClientType)userDto.ClientType == ClientType.Individual) ? null : userDto.CompanyName,
                TaxId = ((ClientType)userDto.ClientType == ClientType.Individual) ? null : userDto.TaxId,
                IsTwoFactorEnabled = userDto.IsTwoFactorEnabled,
            };

            // 2FA setup
            string? qrCode = null;
            if (user.IsTwoFactorEnabled)
            {
                TwoFactorAuthenticator tfa = new();
                user.TwoFactorSecret = SecretGenerator.GenerateSecretKey();
                user.IsTwoFactorReady = false;

                var setupInfo = tfa.GenerateSetupCode(
                    "Marketing System",
                    user.Email,
                    user.TwoFactorSecret,
                    true,
                    3);

                qrCode = setupInfo.QrCodeSetupImageUrl;
            }

            await _unitOfWork.GetUserRepository().Add(user);
            await _unitOfWork.Save();

            return new()
            {
                Email = user.Email,
                IsSuccess = true,
                IsTwoFactorEnabled = user.IsTwoFactorEnabled,
                TwoFactorQrCode = qrCode
            };
        }



        public async Task<bool> RegisterVerify2fa(Verify2faDto verifyDto)
        {
            if (verifyDto.Email is null) return false;

            var user = await _unitOfWork.GetUserRepository().GetByEmailAsync(verifyDto.Email);
            if (user is null) return false;

            TwoFactorAuthenticator tfa = new();
            if (tfa.ValidateTwoFactorPIN(user.TwoFactorSecret, verifyDto.Code, true))
            {
                user.IsTwoFactorReady = true;
                _unitOfWork.GetUserRepository().Update(user);
                await _unitOfWork.Save();
                return true;
            }

            return false;
        }









        public async Task<bool> RegisterAdminOrEmployee(UserDto userDto)
        {

            userDto.Phone = _encryptionService.Encrypt(userDto.Phone);
            userDto.Address = _encryptionService.Encrypt(userDto.Address);
            userDto.Country = _encryptionService.Encrypt(userDto.Country);
            userDto.City = _encryptionService.Encrypt(userDto.City);
            userDto.Firstname = _encryptionService.Encrypt(userDto.Firstname);
            userDto.Lastname = _encryptionService.Encrypt(userDto.Lastname);

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
        public async Task<bool> DeleteDataAsync(long id)
        {
            var adsDeleted = await _unitOfWork.GetAdvertisementRepository().DeleteAdsByClientIdAsync(id);
            var tokenDeleted = await _unitOfWork.GetPasswordlessTokenRepository().DeleteTokenByUserIdAsync(id);
            var requestDeleted = await _unitOfWork.GetRegistrationRequestRepository().DeleteRegistrationRequestByUserIdAsync(id);
            if (adsDeleted && tokenDeleted && requestDeleted)
            {
                return true;
            }
            return false;
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

            var token = GenerateTempEmailToken(email);
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

            return await _emailHandler.SendEmail(email, link, "Passwordless Login");
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

        private string GenerateTempEmailToken(string email, string? timestamp = null)
        {
            if (timestamp is null)
            {
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            }
            var payload = $"{email}:{timestamp}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_hmacConfig.Secret));
            var tokenBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var token = Convert.ToBase64String(tokenBytes);
            var urlSafeToken = token.Replace('+', '-').Replace('/', '_').TrimEnd('=');

            return urlSafeToken;
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
                Firstname = _encryptionService.Decrypt(user.Firstname),
                Lastname = _encryptionService.Decrypt(user.Lastname),
                Address = _encryptionService.Decrypt(user.Address),
                City = _encryptionService.Decrypt(user.City),
                Country = _encryptionService.Decrypt(user.Country),
                Phone = _encryptionService.Decrypt(user.Phone),
                CompanyName = user.CompanyName,
                TaxId = user.TaxId,
                PackageType = (int)user.PackageType, 
                ClientType = (int)user.ClientType, 
                Role = (int)user.Role 
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
                Password = user.Password,
                Firstname = _encryptionService.Decrypt(user.Firstname),
                Lastname = _encryptionService.Decrypt(user.Lastname),
                Address = _encryptionService.Decrypt(user.Address),
                City = _encryptionService.Decrypt(user.City),
                Country = _encryptionService.Decrypt(user.Country),
                Phone = _encryptionService.Decrypt(user.Phone),
                CompanyName = user.CompanyName,
                TaxId = user.TaxId,
                PackageType = (int)user.PackageType, 
                ClientType = (int)user.ClientType, 
                Role = (int)user.Role 
            });
            return userDtos;
        }

        public async Task<IEnumerable<UserDto>> GetUnblocked()
        {
            var users = await _unitOfWork.GetUserRepository().GetAll();

            var userDtos = users
                .Where(user => user.AccountStatus != AccountStatus.Blocked) 
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Password = user.Password,
                    Firstname = _encryptionService.Decrypt(user.Firstname),
                    Lastname = _encryptionService.Decrypt(user.Lastname),
                    Address = _encryptionService.Decrypt(user.Address),
                    City = _encryptionService.Decrypt(user.City),
                    Country = _encryptionService.Decrypt(user.Country),
                    Phone = _encryptionService.Decrypt(user.Phone),
                    CompanyName = user.CompanyName,
                    TaxId = user.TaxId,
                    PackageType = (int)user.PackageType,
                    ClientType = (int)user.ClientType,
                    Role = (int)user.Role
                });
            return userDtos; 

        }

        // TO-DO: promeniti zbog dodatog sifrovanja osetljivih podataka
        public async Task<bool> UpdateUser(UserDto user)
        {
            var userToUpdate = await _unitOfWork.GetUserRepository().GetByIdAsync(user.Id);
            if (userToUpdate == null)
            {
                return false;
            }

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

        public async Task<bool> ChangePassword(ChangePasswordRequestDto request)
        {
            var userToUpdate = await _unitOfWork.GetUserRepository().GetByIdAsync(request.UserId);
            if (userToUpdate == null)
            {
                return false; // User not found
            }

            var oldPasswordHashed = _unitOfWork.GetPasswordHasher().HashPassword(request.OldPassword);

            if (!_unitOfWork.GetPasswordHasher().VerifyPassword(request.OldPassword, userToUpdate.Password))
            {
                return false;
            }


            userToUpdate.Password = _unitOfWork.GetPasswordHasher().HashPassword(request.NewPassword);

            _unitOfWork.GetUserRepository().Update(userToUpdate);
            await _unitOfWork.Save();

            return true;
        }
        public async Task<bool> BlockUser(UserDto user)
        {
            var userToUpdate = await _unitOfWork.GetUserRepository().GetByIdAsync(user.Id);
            if (userToUpdate == null)
            {
                return false; // User not found
            }

            userToUpdate.AccountStatus = AccountStatus.Blocked;

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

        public async Task<bool?> CreateRegistrationRequestAsync(UserDto user)
        {
            user.Id = (await _unitOfWork.GetUserRepository().GetByEmailAsync(user.Email)).Id;

            var request = new RegistrationRequest
            {
                UserId = user.Id,
                Email = user.Email,
                RegistrationDate = DateTime.UtcNow,
                Status = DA.Contracts.Model.RegistrationRequestStatus.Pending,
                Token = null,
                TokenExpirationDate = null
            };
            await _unitOfWork.GetRegistrationRequestRepository().Add(request);
            await _unitOfWork.Save();

            return true;
        }
        public async Task<bool> ActivateAccount(string token)
        {
            //RxiyYwhZ/yYXLX9tPd3cQnLRD/lBj++zX4w8krdpoec=
            //RxiyYwhZ/yYXLX9tPd3cQnLRD/lBjzX4w8krdpoec=
            var registrationRequest = await _unitOfWork.GetRegistrationRequestRepository().GetByTokenAsync(token);
            if (registrationRequest is null ||
                registrationRequest.Status != DA.Contracts.Model.RegistrationRequestStatus.Approved ||
                registrationRequest.TokenExpirationDate < DateTime.UtcNow ||
                registrationRequest.UserId is null)
            {
                return false;
            }

            try
            {
                var user = await _unitOfWork.GetUserRepository().GetByIdAsync(registrationRequest.UserId ?? throw new Exception("User for this registration request does not exist."));
                user.AccountStatus = AccountStatus.Active;
                _unitOfWork.GetUserRepository().Update(user);
                await _unitOfWork.Save();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ApproveRegisterRequestAsync(int requestId)
        {
            var request = await _unitOfWork.GetRegistrationRequestRepository().GetByIdAsync(requestId);
            if (request is null)
            {
                return false;
            }

            var user = await _unitOfWork.GetUserRepository().GetByEmailAsync(request.Email);
            if (user is null)
            {
                return false;
            }

            var token = GenerateTempEmailToken(user.Email);
            // HERE
            var link = $"http://localhost:4200/authenticate-registration-request?token={token}"; // TODO: Adjust the link

            request.Status = DA.Contracts.Model.RegistrationRequestStatus.Approved;
            request.Token = token;
            request.TokenExpirationDate = DateTime.UtcNow.AddHours(24);

            _unitOfWork.GetRegistrationRequestRepository().Update(request);
            await _unitOfWork.Save();

            return await _emailHandler.SendEmail(user.Email, $"<p>Please verify your account using the following <a href=\"{link}\">link</a>.</p>", "Registration Approved!");
        }

        public async Task<bool> RejectRegisterRequestAsync(int requestId, string reason)
        {
            var request = await _unitOfWork.GetRegistrationRequestRepository().GetByIdAsync(requestId);
            if (request is null)
            {
                return false;
            }

            var user = await _unitOfWork.GetUserRepository().GetByEmailAsync(request.Email);
            if (user is null)
            {
                return false;
            }

            request.Status = DA.Contracts.Model.RegistrationRequestStatus.Rejected;
            request.Reason = reason;
            request.Token = null;
            request.TokenExpirationDate = null;
            request.UserId = null;

            _unitOfWork.GetRegistrationRequestRepository().Update(request);
            _unitOfWork.GetUserRepository().Delete(user.Id);

            await _unitOfWork.Save();

            return await _emailHandler.SendEmail(user.Email, $"<p>Administrator decided to reject your registration request.</p> {(request.Reason is null ? "" : $"Reason: {request.Reason}")}", "Registration Rejected");
        }

        public async Task<IEnumerable<RegistrationRequestDto>> GetAllRegistrationRequestsAsync()
        {
            var requests = await _unitOfWork.GetRegistrationRequestRepository().GetAll();

            var requestDtos = requests.ToList().Select(req => new RegistrationRequestDto
            {
                Id = req.Id,
                UserId = req.UserId,
                Email = req.Email,
                RegistrationDate = req.RegistrationDate,
                Status = (Contracts.DTO.RegistrationRequestStatus)req.Status,
                Reason = req.Reason
            });

            return requestDtos;
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email)
        {

            var user = await _unitOfWork.GetUserRepository().GetByEmailAsync(email);
            if (user == null) return false;
  

            var token = GenerateTempEmailToken(email);
            var link = $"http://localhost:4200/reset-password?token={token}";

            await _unitOfWork.GetPasswordResetTokenRepository().Add(
                new PasswordResetToken()
                {
                    Token = token,
                    ExpirationDate = DateTime.UtcNow.AddHours(1),
                    IsUsed = false,
                    UserId = user.Id
                });
            await _unitOfWork.Save();

            return await _emailHandler.SendEmail(email, link, "Password Reset");
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var resetToken = await _unitOfWork.GetPasswordResetTokenRepository().GetByTokenAsync(token);
            if (resetToken == null || resetToken.IsUsed || resetToken.ExpirationDate < DateTime.UtcNow)
            {
                return false;
            }
            var user = await _unitOfWork.GetUserRepository().GetByIdAsync(resetToken.UserId);
            if (user == null) return false;

            user.Password = _unitOfWork.GetPasswordHasher().HashPassword(newPassword);
            _unitOfWork.GetUserRepository().Update(user);
            resetToken.IsUsed = true;
            await _unitOfWork.GetPasswordResetTokenRepository().MarkAsUsedAsync(token);
            await _unitOfWork.Save();

            return true;
        }

        public async Task<string> GetAllLogs()
        {
            string appDirectory = "C:\\Users\\mbovan\\Desktop\\Bezbjednost\\projekat\\bsep-ra-2024-kt2-tim-3\\Marketing_system\\logs\\";
            string logFilePath = Path.Combine(appDirectory, $"log-{DateTime.Now.ToString("yyyyMMdd")}.txt");

            try
            {
                // Proveri da li datoteka postoji
                if (!File.Exists(logFilePath))
                {
                    throw new FileNotFoundException($"Log file not found: {logFilePath}");
                }

                using FileStream fs = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using StreamReader sr = new StreamReader(fs);

                string logs = await sr.ReadToEndAsync();

                var logEntries = logs.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                var stringBuilder = new StringBuilder();
                stringBuilder.Append("[");
                for (int i = 0; i < logEntries.Length; i++)
                {
                    stringBuilder.Append(logEntries[i]);
                    if (i < logEntries.Length - 1)
                    {
                        stringBuilder.Append(",");
                    }
                }
                stringBuilder.Append("]");

                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading log file: {ex.Message}");
                throw;
            }
        }

    }
}
