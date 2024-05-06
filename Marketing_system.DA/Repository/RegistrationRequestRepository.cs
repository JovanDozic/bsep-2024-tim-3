using Marketing_system.DA.Contexts;
using Marketing_system.DA.Contracts.IRepository;
using Marketing_system.DA.Contracts.Model;

namespace Marketing_system.DA.Repository
{
    public class RegistrationRequestRepository : Repository<RegistrationRequest>, IRegistrationRequestRepository
    {
        public DataContext Context
        {
            get { return _dbContext as DataContext; }
        }

        public RegistrationRequestRepository(DataContext context) : base(context) { }

        public new IEnumerable<RegistrationRequest> GetAll()
        {
            return _dbContext.Set<RegistrationRequest>().ToList();
        }
    }
}
