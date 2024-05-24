using Marketing_system.BL.Contracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketing_system.BL.Contracts.IService
{
    public interface IAdvertisementService
    {
        Task<bool> CreateAdvertisement(AdvertisementDto ad);
        Task<IEnumerable<AdvertisementDto>> GetAllAdvertisements();
        Task<bool> UpdateAdvertisement(AdvertisementDto ad);
        Task<bool> DeleteAdsByClientId(long id);

    }
}
