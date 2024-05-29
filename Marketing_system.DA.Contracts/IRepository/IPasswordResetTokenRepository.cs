using Marketing_system.DA.Contracts.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketing_system.DA.Contracts.IRepository
{
    public interface IPasswordResetTokenRepository : IRepository<PasswordResetToken>
    {
        Task<PasswordResetToken> GetByTokenAsync(string token);
        Task MarkAsUsedAsync(string token);
    }
}
