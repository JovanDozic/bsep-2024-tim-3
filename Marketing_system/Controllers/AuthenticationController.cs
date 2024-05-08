using Marketing_system.BL.Contracts.DTO;
using Marketing_system.BL.Contracts.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketing_system.Controllers;
[Route("api/authentication")]
public class AuthenticationController : Controller
{
    private readonly IAuthenticationService _authenticationService;
    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<bool>> RegisterUser([FromBody] UserDto user)
    {
        var isRegistered = await _authenticationService.RegisterUser(user);
        if (isRegistered)
            return Ok(isRegistered);
        return BadRequest(!isRegistered);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticationTokensDto>> Login(string email, string password)
    {
        var token = await _authenticationService.Login(email, password);
        if (token == null)
            return BadRequest(token);
        return Ok(token);
    }

    [HttpPost("updateAccess")]
    [Authorize]
    public async Task<ActionResult<string>> UpdateAccessToken([FromBody] int userId)
    {
        var token = await _authenticationService.UpdateAccessToken(userId);
        if (token == null)
            return BadRequest(token);
        return Ok(token);
    }

    [HttpPost("validateRefresh")]
    public async Task<ActionResult<bool>> ValidateRefreshToken([FromBody] int userId, string refreshToken)
    {
        var token = await _authenticationService.ValidateRefreshToken(userId, refreshToken);
        if (!token)
            return BadRequest(token);
        return Ok(token);
    }

    [HttpPost("validateAccess")]
    [Authorize]
    public async Task<ActionResult<bool>> ValidateAccessToken([FromBody] string accessToken)
    {
        var token = await _authenticationService.ValidateAccessToken(accessToken);
        if (!token)
            return BadRequest(token);
        return Ok(token);
    }

    [HttpPost("requestPasswordlessLogin")]
    public async Task<ActionResult<AuthenticationTokensDto>> RequestPasswordlessLogin(string email)
    {
        var result = await _authenticationService.SendPasswordlessLogin(email);
        if (result == null)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("authenticatePasswordlessLogin")]
    public async Task<ActionResult<AuthenticationTokensDto>> AuthenticatePasswordlessToken([FromQuery] string token)
    {
        var result = await _authenticationService.AuthenticatePasswordlessTokenAsync(token);
        if (result == null)
            return BadRequest(result);
        return Ok(result);
    }
}
