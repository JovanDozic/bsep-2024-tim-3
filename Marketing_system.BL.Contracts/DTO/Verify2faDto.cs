namespace Marketing_system.BL.Contracts.DTO
{
    public class Verify2faDto
    {
        public string? Email { get; set; }
        public string? TempToken { get; set; }
        public string Code { get; set; }
    }
}
