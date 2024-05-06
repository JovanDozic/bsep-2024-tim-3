namespace Marketing_system.BL.Contracts.IService
{
    public interface IPasswordHasher
    {
        string HashPassword(string password, out string salt);
        byte[] GenerateSalt();
        bool VerifyPassword(string password, string salt, string hashedPassword);
        byte[] CombineBytes(byte[] first, byte[] second);

    }
}
