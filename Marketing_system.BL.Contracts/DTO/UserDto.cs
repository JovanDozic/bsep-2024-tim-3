namespace Marketing_system.BL.Contracts.DTO
{
    public class UserDto
    {
        public int Id { get; set; } = 0;
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string? CompanyName { get; set; }
        public string? TaxId { get; set; }
        public int PackageType { get; set; }
        public int ClientType { get; set; }
        public int Role { get; set; }
        public bool IsTwoFactorEnabled { get; set; } = false;
    }
}
