using Marketing_system.BL.Contracts.DTO;

namespace Marketing_system.BL.Contracts.IService
{
    public interface IAuthenticationService
    {
        Task<bool> RegisterUser(UserDto userDto);
        Task<AuthenticationTokensDto?> Login(string email, string password);
        Task<string> UpdateAccessToken(int userId);
        Task<bool> ValidateRefreshToken(int userId, string refreshToken);
        Task<bool> ValidateAccessToken(string accessToken);
        Task<bool?> SendPasswordlessLogin(string email);
        Task<AuthenticationTokensDto?> AuthenticatePasswordlessTokenAsync(string token);

    }
}
