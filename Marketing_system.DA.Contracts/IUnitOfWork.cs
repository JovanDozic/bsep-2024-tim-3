using Marketing_system.DA.Contracts.IRepository;

namespace Marketing_system.DA.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> Save();
        public IUserRepository GetUserRepository();
        public ITokenGeneratorRepository GetTokenGeneratorRepository();
        public IPasswordHasher GetPasswordHasher();
        public IRegistrationRequestRepository GetRegistrationRequestRepository();
        public IPasswordlessTokenRepository GetPasswordlessTokenRepository();
        public IAdvertisementRepository GetAdvertisementRepository();
        public IRoleRepository GetRoleRepository();
        public IPasswordResetTokenRepository GetPasswordResetTokenRepository();
    }
}
