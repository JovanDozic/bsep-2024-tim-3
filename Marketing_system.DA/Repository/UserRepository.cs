using Marketing_system.DA.Contexts;
using Marketing_system.DA.Contracts.IRepository;
using Marketing_system.DA.Contracts.Model;
using Marketing_system.DA.Contracts.Shared;
using Microsoft.EntityFrameworkCore;
using Twilio.Http;
using static QRCoder.PayloadGenerator;

namespace Marketing_system.DA.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public DataContext Context
        {
            get { return _dbContext as DataContext; }
        }

        public UserRepository(DataContext context) : base(context)
        {

        }
        public new IEnumerable<User> GetAll()
        {
            return _dbContext.Set<User>().ToList();
        }
        /*public bool CheckPasswordAsync(User? user, string password)
        {
            throw new NotImplementedException();
        }*/

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _dbContext.Set<User>().FirstOrDefaultAsync(x => x.Email == email);
        }

        public string? GetPasswordByEmail(string email)
        {
            var user = _dbContext.Set<User>().SingleOrDefault(x => x.Email == email);
            if (user == null) { return null; }
            return user.Password;
        }

        public async Task<List<User>> GetAdmins()
        {
            return await _dbContext.Set<User>().Where(user => user.Role == UserRole.Admin).ToListAsync();
        }

        public async Task<bool> DeleteUserByIdAsync(long id)
        {
            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(x => x.Id == id);
            if(user != null)
            {
                try
                {
                    _dbContext.Set<User>().Remove(user);
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
