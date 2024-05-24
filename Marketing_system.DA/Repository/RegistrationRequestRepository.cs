using Marketing_system.DA.Contexts;
using Marketing_system.DA.Contracts.IRepository;
using Marketing_system.DA.Contracts.Model;
using Microsoft.EntityFrameworkCore;

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

        public async Task<RegistrationRequest> GetByTokenAsync(string token)
        {
            return await _dbContext.Set<RegistrationRequest>().FirstOrDefaultAsync(x => x.Token == token);
        }

        public IEnumerable<RegistrationRequest> GetAllByEmailAsync(string email)
        {
            return _dbContext.Set<RegistrationRequest>().Where(x => x.Email == email).ToList();
        }

        public async Task<bool> DeleteRegistrationRequestByUserIdAsync(long id)
        {
            var request = await _dbContext.Set<RegistrationRequest>().FirstOrDefaultAsync(x => x.UserId == id);
            if (request != null)
            {
                try
                {
                    _dbContext.Set<RegistrationRequest>().Remove(request);
                    await _dbContext.SaveChangesAsync();
                    return true;

                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
