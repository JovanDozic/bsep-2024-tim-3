﻿using Marketing_system.BL.Contracts.DTO;
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
        bool isRegistered;
        if (user.Role == 0)
        {
            isRegistered = await _authenticationService.RegisterUser(user);
        }
        else
        {
            isRegistered = await _authenticationService.RegisterAdminOrEmployee(user);
        }

        // TODO: call request service to send email verification



        return isRegistered ? Ok(isRegistered) : BadRequest(!isRegistered);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticationResponseDTO>> Login([FromBody] CredentialsDto credentialsDto)
    {
        var token = await _authenticationService.Login(credentialsDto.Username, credentialsDto.Password);
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
    [AllowAnonymous]
    public async Task<ActionResult<TokensDto>> RequestPasswordlessLogin([FromBody] CredentialsDto credentialsDto)
    {
        var result = await _authenticationService.SendPasswordlessLogin(credentialsDto.Username);
        if (result == null)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("authenticatePasswordlessLogin")]
    [AllowAnonymous]
    public async Task<ActionResult<TokensDto>> AuthenticatePasswordlessToken([FromBody] PasswordlessTokenDto token)
    {
        var result = await _authenticationService.AuthenticatePasswordlessTokenAsync(token.Token);
        if (result == null)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("getUser/{id:int}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _authenticationService.GetUserById(id);
        if (user == null)
            return NotFound(); // User not found

        return Ok(user);
    }
    [HttpGet("getAllUsers")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _authenticationService.GetAllUsers();
        return Ok(users);
    }
    [HttpPost("updateUser")]
    public async Task<ActionResult<bool>> UpdateUser([FromBody] UserDto user)
    {
        var isUpdated = await _authenticationService.UpdateUser(user);
        if (isUpdated)
        {
            return Ok(isUpdated);
        }
        return NotFound(); // Return appropriate status code if user not found
    }
}
