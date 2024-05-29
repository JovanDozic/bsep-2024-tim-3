namespace Marketing_system.BL.Contracts.DTO
{
    public class TokensDto
    {
        public int Id { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        public string? TempToken { get; set; }
    }
}
