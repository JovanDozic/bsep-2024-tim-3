using Marketing_system.BL.Contracts.IService;
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
    }
}
