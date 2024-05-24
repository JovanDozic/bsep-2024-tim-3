using Marketing_system.DA.Contracts.Model;

namespace Marketing_system.DA.Contracts.IRepository
{
    public interface IPasswordlessTokenRepository : IRepository<PasswordlessToken>
    {
        Task<PasswordlessToken> GetByTokenAsync(string token);

        Task<bool> DeleteTokenByUserIdAsync(long iUserId);
    }
}
