#nullable disable

namespace Marketing_system.BL.Contracts.DTO
{
    public class ReCAPTCHAResponse
    {
        public bool Success { get; set; }
        public string Challenge_ts { get; set; }
        public string Hostname { get; set; }
        public string[] ErrorCodes { get; set; }
    }
}
