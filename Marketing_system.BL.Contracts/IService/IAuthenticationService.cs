using Marketing_system.BL.Contracts.DTO;

namespace Marketing_system.BL.Contracts.IService
{
    public interface IAuthenticationService
    {
        Task<AuthenticationTokensDto> RegisterUser(UserDto userDto);
    }
}
