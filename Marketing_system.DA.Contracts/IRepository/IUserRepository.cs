using Marketing_system.DA.Contracts.Model;

namespace Marketing_system.DA.Contracts.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        string GetPasswordByEmail(string email);
        Task<List<User>> GetAdmins();
    }
}
