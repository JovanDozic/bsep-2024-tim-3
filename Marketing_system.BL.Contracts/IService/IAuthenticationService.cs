using Marketing_system.BL.Contracts.DTO;

namespace Marketing_system.BL.Contracts.IService
{
    public interface IAuthenticationService
    {
        Task<bool?> RegisterUser(UserDto userDto);
        Task<AuthenticationTokensDto> Login(string email, string password);
    }
}
