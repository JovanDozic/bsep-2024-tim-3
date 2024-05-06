using Marketing_system.DA.Contracts.Model;

namespace Marketing_system.DA.Contracts.IRepository
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        /*bool CheckPasswordAsync(User? user, string password);*/
    }
}
