using Marketing_system.BL.Contracts.DTO;

namespace Marketing_system.BL.Contracts.IService
{
    public interface IAuthenticationService
    {
        Task<RegistrationResponseDto> RegisterUser(UserDto userDto);
        Task<TokensDto?> Login(string email, string password);
        Task<string> UpdateAccessToken(int userId);
        Task<bool> ValidateRefreshToken(int userId, string refreshToken);
        Task<bool> ValidateAccessToken(string accessToken);
        Task<bool?> SendPasswordlessLogin(string email);
        Task<TokensDto?> AuthenticatePasswordlessTokenAsync(string token);
        Task<UserDto> GetUserById(int userId);
        Task<IEnumerable<UserDto>> GetAllUsers();
        Task<bool> UpdateUser(UserDto user);
        Task<bool> RegisterAdminOrEmployee(UserDto userDto);
        public Task<bool?> CreateRegistrationRequestAsync(UserDto user);
        public Task<bool> ActivateAccount(string token);
        public Task<bool> ApproveRegisterRequestAsync(int requestId);
        public Task<bool> RejectRegisterRequestAsync(int requestId, string reason);
        public Task<IEnumerable<RegistrationRequestDto>> GetAllRegistrationRequestsAsync();
        public Task<bool> DeleteDataAsync(long id);
        Task<bool> BlockUser(UserDto user);
        Task<IEnumerable<UserDto>> GetUnblocked();
        Task<bool> ChangePassword(ChangePasswordRequestDto request);
        Task<bool> SendPasswordResetEmailAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
        Task<bool> RegisterVerify2fa(Verify2faDto verifyDto);
        public Task<TokensDto?> LoginVerify2fa(Verify2faDto verifyDto);
        Task<string> GetAllLogs();
    }
}
