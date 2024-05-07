using Marketing_system.DA.Contracts.IRepository;
using System.Security.Cryptography;
using System.Text;

namespace Marketing_system.DA.Repository
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            return hashedPassword;
        }

        public bool VerifyPassword(string enteredPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, hashedPassword);
        }
    }
}
