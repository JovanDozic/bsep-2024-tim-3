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
    public class AdvertisementRepository : Repository<Advertisement>, IAdvertisementRepository
    {
        public DataContext Context
        {
            get { return _dbContext as DataContext; }
        }
        public AdvertisementRepository(DataContext context) : base(context)
        {

        }
        public async Task<bool> DeleteAdsByClientIdAsync(long id)
        {
            var adsToRemove = await _dbContext.Set<Advertisement>().Where(a => a.ClientId == id).ToListAsync();
            if (adsToRemove != null)
            {
                try
                {
                    foreach(var ad in adsToRemove)
                    {
                        _dbContext.Set<Advertisement>().Remove(ad);
                    }
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
