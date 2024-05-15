namespace Marketing_system.BL.Contracts.DTO
{
    public enum RegistrationRequestStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class RegistrationRequestDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public RegistrationRequestStatus Status { get; set; }
        public string? Reason { get; set; }
        public DateTime? TokenExpirationDate { get; set; }
    }
}
