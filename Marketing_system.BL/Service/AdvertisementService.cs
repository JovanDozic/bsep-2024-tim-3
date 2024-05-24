using Marketing_system.BL.Contracts.DTO;
using Marketing_system.BL.Contracts.IService;
using Marketing_system.DA.Contracts;
using Marketing_system.DA.Contracts.Model;
using Marketing_system.DA.Contracts.Shared;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketing_system.BL.Service
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdvertisementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateAdvertisement(AdvertisementDto ad)
        {
            await _unitOfWork.GetAdvertisementRepository().Add(new Advertisement(ad.Slogan, ad.StartDate, ad.EndDate, ad.Description, ad.ClientId, ad.Deadline, AdvertisementStatus.Request));
            await _unitOfWork.Save();
            return true;
        }

        public async Task<IEnumerable<AdvertisementDto>> GetAllAdvertisements()
        {
            var ads = await _unitOfWork.GetAdvertisementRepository().GetAll();
            var adDtos = ads.Select(ad => new AdvertisementDto
            {
                Id = ad.Id,
                Slogan = ad.Slogan,
                StartDate = ad.StartDate,
                EndDate = ad.EndDate,
                Description = ad.Description,
                ClientId = ad.ClientId,
                Deadline = ad.Deadline,
                Status = (int)ad.Status
            });
            return adDtos;
        }

        public async Task<bool> UpdateAdvertisement(AdvertisementDto ad)
        {
           var adToUpdate = await _unitOfWork.GetAdvertisementRepository().GetByIdAsync(ad.Id);
            if (adToUpdate == null)
            {
                return false;
            }
            adToUpdate.Slogan = ad.Slogan;
            adToUpdate.StartDate = ad.StartDate;
            adToUpdate.EndDate = ad.EndDate;
            adToUpdate.Description = ad.Description;
            adToUpdate.ClientId = ad.ClientId;
            adToUpdate.Deadline = ad.Deadline;
            adToUpdate.Status = (AdvertisementStatus)ad.Status;

            _unitOfWork.GetAdvertisementRepository().Update(adToUpdate);
            await _unitOfWork.Save();

            return true;
        }
        public async Task<bool> DeleteAdsByClientId(long id)
        {
            return await _unitOfWork.GetAdvertisementRepository().DeleteAdsByClientIdAsync(id);
        }
    }
}
