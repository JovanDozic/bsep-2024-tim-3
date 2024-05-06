using Marketing_system.DA.Contracts.Shared;

namespace Marketing_system.DA.Contracts.Model
{
    public class RegistrationRequest : Entity
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDateTime { get; set; }
        public AccountStatus AccountStatus { get; set; }
        public PackageType PackageType { get; set; }

        public RegistrationRequest(string firstname, string lastname, string email, DateTime dateTime, PackageType packageType)
        {
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
            RegistrationDateTime = dateTime;
            AccountStatus = AccountStatus.Requested;
            PackageType = packageType;
        }
    }
}
