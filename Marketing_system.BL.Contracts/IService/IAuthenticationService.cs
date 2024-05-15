using Marketing_system.BL.Contracts.DTO;

namespace Marketing_system.BL.Contracts.IService
{
    public interface IAuthenticationService
    {
        Task<bool> RegisterUser(UserDto userDto);
        Task<AuthenticationResponseDTO?> Login(string email, string password);
        Task<string> UpdateAccessToken(int userId);
        Task<bool> ValidateRefreshToken(int userId, string refreshToken);
        Task<bool> ValidateAccessToken(string accessToken);
        Task<bool?> SendPasswordlessLogin(string email);
        Task<TokensDto?> AuthenticatePasswordlessTokenAsync(string token);
        Task<UserDto> GetUserById(int userId);
        Task<IEnumerable<UserDto>> GetAllUsers();
        Task<bool> UpdateUser(UserDto user);
        Task<bool> RegisterAdminOrEmployee(UserDto userDto);
        public Task<bool?> CreateRegistrationRequestAsync(UserDto user);
        public Task<bool> ActivateAccount(string token);
    }
}
