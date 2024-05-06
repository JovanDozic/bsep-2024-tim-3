using Marketing_system.BL.Contracts.DTO;
using Marketing_system.BL.Contracts.IService;
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
    public async Task<bool?> RegisterUser([FromBody] UserDto user)
    {
        return await _authenticationService.RegisterUser(user);
    }
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<AuthenticationTokensDto> Login([FromBody] string email, string password)
    {
        return await _authenticationService.Login(email, password);
    }

}
