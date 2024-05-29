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
    public async Task<ActionResult<RegistrationResponseDto>> RegisterUser([FromBody] UserDto user)
    {
        // Specification states: `Potrebno je mogućiti dvofaktorsku prijavu na sistem, gde bi se od klijenta pored lozinke zahtevalo još nešto što “klijent zna ili poseduje.`

        RegistrationResponseDto response;
        if (user.Role == 0)
        {
            response = await _authenticationService.RegisterUser(user);
        }
        else
        {
            var isRegistered = await _authenticationService.RegisterAdminOrEmployee(user);
            response = new RegistrationResponseDto()
            {
                IsSuccess = isRegistered
            };
        }

        if (response.IsSuccess)
        {
            // TODO: Uncomment this:
            // await _authenticationService.CreateRegistrationRequestAsync(user);
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpPost("register/verify2fa")]
    [AllowAnonymous]
    public async Task<ActionResult<RegistrationResponseDto>> RegisterVerify2fa([FromBody] Verify2faDto verifyDto)
    {
        var isSuccess = await _authenticationService.RegisterVerify2fa(verifyDto);
        if (isSuccess)
        {
            return Ok(isSuccess);
        }
        return BadRequest("Two factor code is not correct.");
    }





    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokensDto>> Login([FromBody] CredentialsDto credentialsDto)
    {
        var token = await _authenticationService.Login(credentialsDto.Username, credentialsDto.Password);
        if (token == null)
            return BadRequest(token);
        return Ok(token);
    }

    [HttpPost("login/verify2fa")]
    [AllowAnonymous]
    public async Task<ActionResult<TokensDto>> VerifyLogin()
    {
        throw new NotImplementedException();
        //var token = await _authenticationService.VerifyLogin(verifyLoginDto);
        //if (token == null)
        //    return BadRequest(token);
        //return Ok(token);
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
    [Authorize]
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
    public async Task<ActionResult<TokensDto>> AuthenticatePasswordlessToken([FromBody] EmailTokenDto token)
    {
        var result = await _authenticationService.AuthenticatePasswordlessTokenAsync(token.Token);
        if (result == null)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("getUser/{id:int}")]
    //[Authorize]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _authenticationService.GetUserById(id);
        if (user == null)
            return NotFound(); // User not found

        return Ok(user);
    }

    [HttpGet("getAllUsers")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _authenticationService.GetAllUsers();
        return Ok(users);
    }

    [HttpGet("getUnblocked")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUnblocked()
    {
        var users = await _authenticationService.GetUnblocked();
        return Ok(users);
    }

    [HttpPost("updateUser")]
    //[Authorize]
    public async Task<ActionResult<bool>> UpdateUser([FromBody] UserDto user)
    {
        var isUpdated = await _authenticationService.UpdateUser(user);
        if (isUpdated)
        {
            return Ok(isUpdated);
        }
        return NotFound();
    }

    [HttpPost("changePassword")]
    //[Authorize]
    public async Task<ActionResult<bool>> ChangePassword([FromBody] ChangePasswordRequestDto requestData)
    {
        var isChanged = await _authenticationService.ChangePassword(requestData);
        if (isChanged)
        {
            return Ok(isChanged);
        }
        return NotFound();
    }

    [HttpPost("blockUser")]
    //[Authorize]
    public async Task<ActionResult<bool>> BlockUser([FromBody] UserDto user)
    {
        var isUpdated = await _authenticationService.BlockUser(user);
        if (isUpdated)
        {
            return Ok(isUpdated);
        }
        return NotFound(); // Return appropriate status code if user not found
    }

    [HttpPost("activateAccount")]
    [AllowAnonymous]
    public async Task<ActionResult<bool>> ActivateAccount([FromBody] EmailTokenDto token)
    {
        var isActivated = await _authenticationService.ActivateAccount(token.Token.Replace(" ", ""));
        if (isActivated)
        {
            return Ok(isActivated);
        }
        return BadRequest();
    }

    [HttpPost("approveRequest")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> ApproveRegistrationRequest([FromBody] RegistrationRequestUpdateDto dto)
    {
        var isApproved = await _authenticationService.ApproveRegisterRequestAsync(dto.Id);
        if (isApproved)
        {
            return Ok(isApproved);
        }
        return BadRequest();
    }

    [HttpPost("rejectRequest")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> RejectRegistrationRequest([FromBody] RegistrationRequestUpdateDto dto)
    {
        var isRejected = await _authenticationService.RejectRegisterRequestAsync(dto.Id, dto.Reason ?? string.Empty);
        if (isRejected)
        {
            return Ok(isRejected);
        }
        return BadRequest();
    }

    [HttpDelete("delete-data/{idUser}")]
    [Authorize]
    public async Task<ActionResult<bool>> DeleteData([FromRoute] int idUser)
    {
        var isDeleted = await _authenticationService.DeleteDataAsync(idUser);
        if (isDeleted)
        {
            return Ok(isDeleted);
        }
        return BadRequest();
    }

    [HttpGet("getAllRegistrationRequests")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<RegistrationRequestDto>>> GetAllRegistrationRequests()
    {
        var requests = await _authenticationService.GetAllRegistrationRequestsAsync();
        return Ok(requests);
    }
}
