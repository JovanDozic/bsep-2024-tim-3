using Marketing_system.DA.Contracts.Model;

namespace Marketing_system.DA.Contracts.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        /*bool CheckPasswordAsync(User? user, string password);*/
        string GetSaltByEmail(string email);
        string GetPasswordByEmail(string email);
    }
}
