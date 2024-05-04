using Marketing_system.DA.Contracts.Shared;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Marketing_system.DA.Contracts.Model
{
    public class User : IdentityUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string? CompanyName { get; set; }
        public int? TaxId { get; set; }
        public UserRole Role { get; set; }
        public ClientType? ClientType { get; set; }
        public User(string email, string password, string firstname, string lastname, string address, string city, string country, string phone, UserRole role, ClientType clientType)
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
        }

        public User(string email, string password, string companyName, int taxId, string address, string city, string country, string phone, UserRole role, ClientType clientType)
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
        }


    }
}
