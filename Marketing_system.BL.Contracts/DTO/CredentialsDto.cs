namespace Marketing_system.BL.Contracts.DTO
{
    public class CredentialsDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string? TwoFactorTempToken { get; set; }
        public string? TwoFactorCode { get; set; }
        public string ReCAPTCHAToken { get; set; }

    }
}
