using Marketing_system.DA.Contracts.Shared;

namespace Marketing_system.DA.Contracts.Model
{
    public enum RegistrationRequestStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class RegistrationRequest : Entity
    {
        public int? UserId { get; set; } // TODO: If request is rejected, we will delete user mapped to this request.
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; } // TODO: When user tries to register, we will check if the email exists in the request database, if it does, we will check last registration date, if it is less than 24 hours, we will not allow the user to register.
        public RegistrationRequestStatus Status { get; set; }
        public string? Reason { get; set; }
        public string? Token { get; set; }
        public DateTime? TokenExpirationDate { get; set; }
    }
}
