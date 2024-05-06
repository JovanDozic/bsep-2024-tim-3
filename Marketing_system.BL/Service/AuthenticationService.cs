using Marketing_system.BL.Contracts.DTO;
using Marketing_system.BL.Contracts.IService;
using Marketing_system.DA.Contracts;
using Marketing_system.DA.Contracts.Model;
using Marketing_system.DA.Contracts.Shared;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;



namespace Marketing_system.BL.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _key = Environment.GetEnvironmentVariable("JWT_KEY") ?? "marketingsystem_secret_key";
        private readonly string _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "marketingsystem";
        private readonly string _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "marketingsystem-front.com";

        public AuthenticationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthenticationTokensDto?> Login(string email, string password)
        {
            var user = await _unitOfWork.GetUserRepository().GetByEmailAsync(email);
            if (user == null)
            {
                var hashedPassword = _unitOfWork.GetUserRepository().GetPasswordByEmail(email);
                var storedSalt = _unitOfWork.GetUserRepository().GetSaltByEmail(email);
                if(_unitOfWork.GetPasswordHasher().VerifyPassword(password, hashedPassword, storedSalt))
                { 
                    var tokens = await _unitOfWork.GetTokenGeneratorRepository().GenerateTokens(user);
                    user.RefreshToken = tokens.RefreshToken;
                    _unitOfWork.GetUserRepository().Update(user);
                    return tokens;
                }
            }
            return null;
        }

        public async Task<bool?> RegisterUser(UserDto userDto)
        {
            var userdb = await _unitOfWork.GetUserRepository().GetByEmailAsync(userDto.Email);
            if(userdb != null)
            {
                return false;
            }

            var (password, salt) = _unitOfWork.GetPasswordHasher().HashPassword(userDto.Password);

            if((ClientType)userDto.ClientType == ClientType.Individual)
            {
                await _unitOfWork.GetUserRepository().Add(new User(userDto.Email, password, userDto.Firstname, userDto.Lastname, userDto.Address, userDto.City, userDto.Country, userDto.Phone, (UserRole)userDto.Role, ClientType.Individual, salt, (PackageType)userDto.PackageType, AccountStatus.Requested));
            } else
            {
                await _unitOfWork.GetUserRepository().Add(new User(userDto.Email, password, userDto.CompanyName, userDto.TaxId, userDto.Address, userDto.City, userDto.Country, userDto.Phone, (UserRole)userDto.Role, (ClientType)userDto.ClientType, salt, (PackageType)userDto.PackageType));
            }

            await _unitOfWork.GetRegistrationRequestRepository().Add(new RegistrationRequest(userDto.Firstname, userDto.Lastname, userDto.Email, DateTime.Now, (PackageType)userDto.PackageType));
            return true;
        }
        public async Task<string> UpdateRefreshToken(int userId)
        {
            var user = await _unitOfWork.GetUserRepository().GetByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var newRefreshToken = _unitOfWork.GetTokenGeneratorRepository().CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            _unitOfWork.GetUserRepository().Update(user);

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

    }
}
