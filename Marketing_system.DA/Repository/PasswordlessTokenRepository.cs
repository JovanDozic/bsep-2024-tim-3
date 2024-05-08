using Marketing_system.DA.Contexts;
using Marketing_system.DA.Contracts.IRepository;
using Marketing_system.DA.Contracts.Model;

namespace Marketing_system.DA.Repository
{
    public class PasswordlessTokenRepository : Repository<PasswordlessToken>, IPasswordlessTokenRepository
    {
        public DataContext Context
        {
            get { return _dbContext as DataContext; }
        }

        public PasswordlessTokenRepository(DataContext context) : base(context) { }
    }
}
