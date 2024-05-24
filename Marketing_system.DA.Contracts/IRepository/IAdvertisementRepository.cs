using Marketing_system.DA.Contracts.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketing_system.DA.Contracts.IRepository
{
    public interface IAdvertisementRepository : IRepository<Advertisement>
    {
        Task<bool> DeleteAdsByClientIdAsync(long id);
    }
}
