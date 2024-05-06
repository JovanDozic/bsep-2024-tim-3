using Marketing_system.BL.Contracts.DTO;
using Marketing_system.BL.Contracts.IService;
using Marketing_system.DA.Contracts.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketing_system.Controllers;
[Route("api/users")]
public class AuthenticationController
{
    private readonly IAuthenticationService _authenticationService;
    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<AuthenticationTokensDto> RegisterUser([FromBody] UserDto user)
    {
        return await _authenticationService.RegisterUser(user);
    }

}
