namespace Marketing_system.DA.Contracts.IRepository
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string enteredPassword, string hashedPassword);

    }
}
