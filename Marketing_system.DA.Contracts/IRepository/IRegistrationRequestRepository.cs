using Marketing_system.DA.Contracts.Model;

namespace Marketing_system.DA.Contracts.IRepository
{
    public interface IRegistrationRequestRepository : IRepository<RegistrationRequest>
    {
        public Task<RegistrationRequest> GetByTokenAsync(string token);
        public IEnumerable<RegistrationRequest> GetAllByEmailAsync(string email);
        public Task<bool> DeleteRegistrationRequestByUserIdAsync(long iUserId);
    }
}
