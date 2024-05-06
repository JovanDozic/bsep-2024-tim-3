using Marketing_system.BL.Contracts.IService;
using System.Security.Cryptography;
using System.Text;

namespace Marketing_system.BL.Service
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password, out string salt)
        {
            byte[] saltBytes = GenerateSalt();
            salt = Convert.ToBase64String(saltBytes);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPasswordBytes = CombineBytes(passwordBytes, saltBytes);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(saltedPasswordBytes);
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public bool VerifyPassword(string password, string salt, string hashedPassword)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPasswordBytes = CombineBytes(passwordBytes, saltBytes);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(saltedPasswordBytes);
                string computedHash = Convert.ToBase64String(hashedBytes);
                return computedHash == hashedPassword;
            }
        }

        public byte[] GenerateSalt()
        {
            byte[] salt = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public byte[] CombineBytes(byte[] first, byte[] second)
        {
            byte[] combined = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, combined, 0, first.Length);
            Buffer.BlockCopy(second, 0, combined, first.Length, second.Length);
            return combined;
        }
    }
}
