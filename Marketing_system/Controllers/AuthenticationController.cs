using Marketing_system.BL.Contracts.DTO;
using Marketing_system.BL.Contracts.IService;
using Marketing_system.DA.Contracts.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Marketing_system.Controllers
{
    [Route("api/authentication")]
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IReCAPTCHAService _reCAPTCHAService;

        public AuthenticationController(IAuthenticationService authenticationService, IReCAPTCHAService reCAPTCHAService)
        {
            _authenticationService = authenticationService;
            _reCAPTCHAService = reCAPTCHAService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<RegistrationResponseDto>> RegisterUser([FromBody] UserDto user)
        {
            Log.Information($"Requested registration from IP: {HttpContext.Connection.RemoteIpAddress}");
            if (user is null)
            {
                Log.Warning($"User registration failed: UserDTO is null from IP: {HttpContext.Connection.RemoteIpAddress}");
                return BadRequest(new RegistrationResponseDto()
                {
                    IsSuccess = false,
                    Message = "UserDTO is null."
                });
            }

            RegistrationResponseDto response;
            Log.Information($"Attempting to register user with email: {user.Email} and role: {(UserRole)user.Role}");
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
                Log.Information($"User {response.Email} registered successfully from IP: {HttpContext.Connection.RemoteIpAddress}");
                // TODO: Uncomment this:
                // await _authenticationService.CreateRegistrationRequestAsync(user);
                return Ok(response);
            }
            Log.Warning($"Unsuccessful registration attempt for email: {user.Email} from IP: {HttpContext.Connection.RemoteIpAddress}");
            return BadRequest(response);
        }

        [HttpPost("register/verify2fa")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> RegisterVerify2fa([FromBody] Verify2faDto verifyDto)
        {
            Log.Information($"Requested 2FA registration verification from IP: {HttpContext.Connection.RemoteIpAddress}");
            var isSuccess = await _authenticationService.RegisterVerify2fa(verifyDto);
            if (isSuccess)
            {
                Log.Information($"Successfully verified 2FA from IP: {HttpContext.Connection.RemoteIpAddress}");
                return Ok(isSuccess);
            }
            Log.Warning($"2FA verification failed from IP: {HttpContext.Connection.RemoteIpAddress}");
            return BadRequest("Two factor code is not correct.");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<TokensDto>> Login([FromBody] CredentialsDto credentialsDto)
        {
            Log.Information($"Login attempt for user: {credentialsDto.Username} from IP: {HttpContext.Connection.RemoteIpAddress}");

            var isReCAPTCHAValid = await _reCAPTCHAService.VerifyToken(credentialsDto.ReCAPTCHAToken);

            if (!isReCAPTCHAValid)
            {
                Log.Warning($"Failed reCAPTCHA validation for user: {credentialsDto.Username} from IP: {HttpContext.Connection.RemoteIpAddress}");
                return BadRequest("Invalid reCAPTCHA token");
            }

            var token = await _authenticationService.Login(credentialsDto.Username, credentialsDto.Password);
            if (token == null)
            {
                Log.Warning($"Failed login attempt for user: {credentialsDto.Username} from IP: {HttpContext.Connection.RemoteIpAddress}");
                return BadRequest("Invalid credentials");
            }
            Log.Information($"Successful login for user: {credentialsDto.Username} from IP: {HttpContext.Connection.RemoteIpAddress}");
            return Ok(token);
        }

        [HttpPost("login/verify2fa")]
        [AllowAnonymous]
        public async Task<ActionResult<TokensDto>> VerifyLogin([FromBody] Verify2faDto verifyDto)
        {
            Log.Information($"2FA login verification attempt for user: {verifyDto.Email} from IP: {HttpContext.Connection.RemoteIpAddress}");
            var token = await _authenticationService.LoginVerify2fa(verifyDto);
            if (token == null)
            {
                Log.Warning($"Failed 2FA login verification for user: {verifyDto.Email} from IP: {HttpContext.Connection.RemoteIpAddress}");
                return BadRequest("Invalid 2FA code");
            }
            Log.Information($"Successful 2FA login for user: {verifyDto.Email} from IP: {HttpContext.Connection.RemoteIpAddress}");
            return Ok(token);
        }

        [HttpPost("updateAccess")]
        [Authorize]
        public async Task<ActionResult<string>> UpdateAccessToken([FromBody] int userId)
        {
            Log.Information($"Update access token attempt for user ID: {userId} from IP: {HttpContext.Connection.RemoteIpAddress}");
            var token = await _authenticationService.UpdateAccessToken(userId);
            if (token == null)
            {
                Log.Warning($"Failed to update access token for user ID: {userId} from IP: {HttpContext.Connection.RemoteIpAddress}");
                return BadRequest("Unable to update access token");
            }
            Log.Information($"Successfully updated access token for user ID: {userId} from IP: {HttpContext.Connection.RemoteIpAddress}");
            return Ok(token);
        }

        [HttpPost("validateRefresh")]
        [Authorize]
        public async Task<ActionResult<bool>> ValidateRefreshToken([FromBody] int userId, string refreshToken)
        {
            Log.Information($"Validate refresh token attempt for user ID: {userId} from IP: {HttpContext.Connection.RemoteIpAddress}");
            var isValid = await _authenticationService.ValidateRefreshToken(userId, refreshToken);
            if (!isValid)
            {
                Log.Warning($"Failed to validate refresh token for user ID: {userId} from IP: {HttpContext.Connection.RemoteIpAddress}");
                return BadRequest("Invalid refresh token");
            }
            Log.Information($"Successfully validated refresh token for user ID: {userId} from IP: {HttpContext.Connection.RemoteIpAddress}");
            return Ok(isValid);
        }

        [HttpPost("validateAccess")]
        [Authorize]
        public async Task<ActionResult<bool>> ValidateAccessToken([FromBody] string accessToken)
        {
            Log.Information($"Validate access token attempt from IP: {HttpContext.Connection.RemoteIpAddress}");
            var isValid = await _authenticationService.ValidateAccessToken(accessToken);
            if (!isValid)
            {
                Log.Warning($"Failed to validate access token from IP: {HttpContext.Connection.RemoteIpAddress}");
                return BadRequest("Invalid access token");
            }
            Log.Information($"Successfully validated access token from IP: {HttpContext.Connection.RemoteIpAddress}");
            return Ok(isValid);
        }

        [HttpPost("requestPasswordlessLogin")]
        [AllowAnonymous]
        public async Task<ActionResult<TokensDto>> RequestPasswordlessLogin([FromBody] CredentialsDto credentialsDto)
        {
            Log.Information($"Passwordless login request for user: {credentialsDto.Username} from IP: {HttpContext.Connection.RemoteIpAddress}");
            var result = await _authenticationService.SendPasswordlessLogin(credentialsDto.Username);
            if (result == null)
            {
                Log.Warning($"Failed passwordless login request for user: {credentialsDto.Username} from IP: {HttpContext.Connection.RemoteIpAddress}", credentialsDto.Username);
                return BadRequest("Unable to send passwordless login request");
            }
            Log.Information($"Successfully requested passwordless login for user: {credentialsDto.Username} from IP: {HttpContext.Connection.RemoteIpAddress}", credentialsDto.Username);
            return Ok(result);
        }

        [HttpPost("authenticatePasswordlessLogin")]
        [AllowAnonymous]
        public async Task<ActionResult<TokensDto>> AuthenticatePasswordlessToken([FromBody] EmailTokenDto token)
        {
            Log.Information($"Passwordless login token authentication attempt from IP: {HttpContext.Connection.RemoteIpAddress}");
            var result = await _authenticationService.AuthenticatePasswordlessTokenAsync(token.Token);
            if (result == null)
            {
                Log.Warning($"Failed passwordless login token authentication from IP: {HttpContext.Connection.RemoteIpAddress}");
                return BadRequest("Invalid token");
            }
            Log.Information($"Successfully authenticated passwordless login token from IP: {HttpContext.Connection.RemoteIpAddress}");
            return Ok(result);
        }

        [HttpGet("getUser/{id:int}")]
        //[Authorize]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            Log.Information($"Get user request for user ID: {id} from IP: {HttpContext.Connection.RemoteIpAddress}");
            var user = await _authenticationService.GetUserById(id);
            if (user == null)
            {
                Log.Warning($"User not found for user ID: {id} from IP: {HttpContext.Connection.RemoteIpAddress}");
                return NotFound(); // User not found
            }
            Log.Information($"Successfully retrieved user for user ID: {id} from IP: {HttpContext.Connection.RemoteIpAddress}");
            return Ok(user);
        }

        [HttpGet("getAllUsers")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            Log.Information($"Get all users request from IP: {HttpContext.Connection.RemoteIpAddress}");
            var users = await _authenticationService.GetAllUsers();
            Log.Information($"Successfully retrieved all users from IP: {HttpContext.Connection.RemoteIpAddress}");
            return Ok(users);
        }

        [HttpGet("getUnblocked")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUnblocked()
        {
            Log.Information($"Get unblocked users request from IP: {HttpContext.Connection.RemoteIpAddress}");
            var users = await _authenticationService.GetUnblocked();
            Log.Information($"Successfully retrieved unblocked users from IP: {HttpContext.Connection.RemoteIpAddress}");
            return Ok(users);
        }

        [HttpPost("updateUser")]
        //[Authorize]
        public async Task<ActionResult<bool>> UpdateUser([FromBody] UserDto user)
        {
            Log.Information($"Update user request for user ID: {user.Id} from IP: {HttpContext.Connection.RemoteIpAddress}");
            var isUpdated = await _authenticationService.UpdateUser(user);
            if (isUpdated)
            {
                Log.Information($"Successfully updated user ID: {user.Id} from IP: {HttpContext.Connection.RemoteIpAddress}");
                return Ok(isUpdated);
            }
            Log.Warning($"Failed to update user ID: {user.Id} from IP: {HttpContext.Connection.RemoteIpAddress}");
            return NotFound();
        }

        [HttpPost("changePassword")]
        //[Authorize]
        public async Task<ActionResult<bool>> ChangePassword([FromBody] ChangePasswordRequestDto requestData)
        {
            Log.Information($"Change password request for user ID: {requestData.UserId} from IP: {HttpContext.Connection.RemoteIpAddress}");
            var isChanged = await _authenticationService.ChangePassword(requestData);
            if (isChanged)
            {
                Log.Information($"Successfully changed password for user ID: {requestData.UserId} from IP: {HttpContext.Connection.RemoteIpAddress}");
                return Ok(isChanged);
            }
            Log.Warning($"Failed to change password for user ID: {requestData.UserId} from IP: {HttpContext.Connection.RemoteIpAddress}");
            return NotFound();
        }

        [HttpPost("blockUser")]
        [Authorize]
        public async Task<ActionResult<bool>> BlockUser([FromBody] UserDto user)
        {
            Log.Information($"Requested blocking user with ID: {user.Id} by user: {User?.Identity?.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
            var isUpdated = await _authenticationService.BlockUser(user);
            if (isUpdated)
            {
                Log.Information($"User with ID: {user.Id} blocked successfully by user: {User?.Identity?.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
                return Ok(isUpdated);
            }
            Log.Warning($"Failed to block user with ID: {user.Id} by user: {User?.Identity?.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
            return NotFound("User not found");
        }

        [HttpPost("activateAccount")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> ActivateAccount([FromBody] EmailTokenDto token)
        {
            Log.Information($"Requested account activation for token: {token.Token}");
            var isActivated = await _authenticationService.ActivateAccount(token.Token.Replace(" ", ""));
            if (isActivated)
            {
                Log.Information($"Account activated successfully for token: {token.Token}");
                return Ok(isActivated);
            }
            Log.Warning($"Failed to activate account for token: {token.Token}");
            return BadRequest("Account activation failed");
        }

        [HttpPost("approveRequest")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> ApproveRegistrationRequest([FromBody] RegistrationRequestUpdateDto dto)
        {
            Log.Information($"Requested approval for registration request ID: {dto.Id} by user: {User?.Identity?.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
            var isApproved = await _authenticationService.ApproveRegisterRequestAsync(dto.Id);
            if (isApproved)
            {
                Log.Information($"Registration request ID: {dto.Id} approved by user: {User?.Identity?.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
                return Ok(isApproved);
            }
            Log.Warning($"Failed to approve registration request ID: {dto.Id} by user: {User?.Identity?.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
            return BadRequest("Approval failed");
        }

        [HttpPost("rejectRequest")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> RejectRegistrationRequest([FromBody] RegistrationRequestUpdateDto dto)
        {
            Log.Information($"Requested rejection for registration request ID: {dto.Id} by user: {User?.Identity?.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
            var isRejected = await _authenticationService.RejectRegisterRequestAsync(dto.Id, dto.Reason ?? string.Empty);
            if (isRejected)
            {
                Log.Information($"Registration request ID: {dto.Id} rejected by user: {User?.Identity?.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
                return Ok(isRejected);
            }
            Log.Warning($"Failed to reject registration request ID: {dto.Id} by user: {User?.Identity?.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
            return BadRequest("Rejection failed");
        }

        [HttpDelete("delete-data/{idUser}")]
        [Authorize]
        public async Task<ActionResult<bool>> DeleteData([FromRoute] int idUser)
        {
            Log.Information($"Requested data deletion for user ID: {idUser} by user: {User?.Identity?.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
            var isDeleted = await _authenticationService.DeleteDataAsync(idUser);
            if (isDeleted)
            {
                Log.Information($"Data deleted successfully for user ID: {idUser} by user: {User?.Identity?.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
                return Ok(isDeleted);
            }
            Log.Warning($"Failed to delete data for user ID: {idUser} by user: {User?.Identity?.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
            return BadRequest("Data deletion failed");
        }

        [HttpGet("getAllRegistrationRequests")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<RegistrationRequestDto>>> GetAllRegistrationRequests()
        {
            Log.Information($"Requested all registration requests by user: {User?.Identity?.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
            var requests = await _authenticationService.GetAllRegistrationRequestsAsync();
            Log.Information($"All registration requests returned successfully for user: {User?.Identity?.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
            return Ok(requests);
        }
        [HttpGet("getAllLogs")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllLogs()
        {
            Log.Information($"Requested all logs by user: {User?.Identity?.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
            var requests = await _authenticationService.GetAllLogs();
            Log.Information($"All registration requests returned successfully for user: {User?.Identity?.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
            return Ok(requests);
        }
        [HttpPost("requestPasswordReset")]
        public async Task<ActionResult<bool>> RequestPasswordReset([FromBody] ForgotPasswordDto emailDto)
        {

            var result = await _authenticationService.SendPasswordResetEmailAsync(emailDto.Email);
            if (!result)
                return BadRequest(result);
            return Ok(result);

        }

        [HttpPost("resetPassword")]
        public async Task<ActionResult<bool>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var result = await _authenticationService.ResetPasswordAsync(resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (!result)
                return BadRequest(result);
            return Ok(result);
        }
    }

}
