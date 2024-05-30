using Marketing_system.DA.Contexts;
using Marketing_system.DA.Contracts.IRepository;
using Marketing_system.DA.Contracts.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketing_system.DA.Repository
{
    public class PasswordResetTokenRepository : Repository<PasswordResetToken>, IPasswordResetTokenRepository
    {
        public DataContext Context
        {
            get { return _dbContext as DataContext; }
        }
        public PasswordResetTokenRepository(DataContext context) : base(context) { }



        public async Task<PasswordResetToken> GetByTokenAsync(string token)
        {
            return await _dbContext.Set<PasswordResetToken>().FirstOrDefaultAsync(x => x.Token == token);
        }

        public async Task MarkAsUsedAsync(string token)
        {
            var tokenEntity = await _dbContext.Set<PasswordResetToken>().FirstOrDefaultAsync(t => t.Token == token);
            if (tokenEntity != null)
            {
                tokenEntity.IsUsed = true;
                _dbContext.Update(tokenEntity);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
