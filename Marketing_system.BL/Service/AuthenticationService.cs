using Marketing_system.BL.Contracts.DTO;
using Marketing_system.BL.Contracts.IService;

namespace Marketing_system.BL.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        public async Task<AuthenticationTokensDto> RegisterUser(UserDto userDto)
        {
            throw new NotImplementedException();
        }
    }
}
