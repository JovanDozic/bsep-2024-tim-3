using Marketing_system.DA.Contexts;
using Marketing_system.DA.Contracts.IRepository;
using Marketing_system.DA.Contracts.Model;
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
    }
}
