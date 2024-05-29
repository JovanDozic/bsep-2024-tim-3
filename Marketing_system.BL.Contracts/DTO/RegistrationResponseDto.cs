namespace Marketing_system.BL.Contracts.DTO
{
    public class RegistrationResponseDto
    {
        public string? Email { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public bool? IsTwoFactorEnabled { get; set; }
        public string? TwoFactorQrCode { get; set; }
    }
}
