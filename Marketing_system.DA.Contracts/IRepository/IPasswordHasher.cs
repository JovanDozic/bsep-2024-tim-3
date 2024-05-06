namespace Marketing_system.DA.Contracts.IRepository
{
    public interface IPasswordHasher
    {
        (string hashedPassword, string salt) HashPassword(string password);
        byte[] GenerateSalt();
        bool VerifyPassword(string password, string salt, string hashedPassword);
        byte[] CombineBytes(byte[] first, byte[] second);

    }
}
