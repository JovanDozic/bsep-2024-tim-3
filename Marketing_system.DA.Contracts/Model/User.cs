using Marketing_system.DA.Contracts.Shared;
using Microsoft.AspNetCore.Identity;

namespace Marketing_system.DA.Contracts.Model
{
    public class User : Entity
    {
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
        public UserRole Role { get; set; }
        public ClientType ClientType { get; set; }
        public string Salt { get; set; }
        public PackageType PackageType { get; set; }
        public AccountStatus AccountStatus { get; set; }
        public string? RefreshToken { get; set; }
        public User(string email, string password, string firstname, string lastname, string address, string city, string country, string phone, UserRole role, ClientType clientType, string salt, PackageType packageType, AccountStatus status)
        {
            Email = email;
            Password = password;
            Firstname = firstname;
            Lastname = lastname;
            Address = address;
            City = city;
            Country = country;
            Phone = phone;
            Role = role;
            ClientType = clientType;
            Salt = salt;
            PackageType = packageType;
            AccountStatus = status;
        }

        public User(string email, string password, string companyName, string? taxId, string address, string city, string country, string phone, UserRole role, ClientType clientType, string salt, PackageType packageType)
        {
            Email = email;
            Password = password;
            CompanyName = companyName;
            TaxId = taxId;
            Address = address;
            City = city;
            Country = country;
            Phone = phone;
            Role = role;
            ClientType = clientType;
            Salt = salt;
            PackageType = packageType;
            AccountStatus = AccountStatus.Requested;
        }

        /*private static UserRole userRoleConverter(int role)
        {
            return role switch
            {
                0 => UserRole.Client,
                1 => UserRole.Employee,
                _ => UserRole.Admin,
            };
        }

        private static ClientType clientTypeConverter(int type)
        {
            return type switch
            {
                0 => Shared.ClientType.Individual,
                _ => Shared.ClientType.Legal_entity,
            };
        }

        private static PackageType packageTypeConverter(int type)
        {
            return type switch
            {
                0 => PackageType.Basic,
                1 => PackageType.Standard,
                _ => PackageType.Golden,
            };
        }*/
    }
}
