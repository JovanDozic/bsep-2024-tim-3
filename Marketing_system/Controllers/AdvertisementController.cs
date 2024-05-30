using Marketing_system.BL.Contracts.DTO;
using Marketing_system.BL.Contracts.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Marketing_system.Controllers
{
    [Route("api/advertisements")]
    public class AdvertisementController : Controller
    {
        private readonly IAdvertisementService _advertisementService;

        public AdvertisementController(IAdvertisementService advertisementService)
        {
            _advertisementService = advertisementService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<bool>> CreateAdvertisement([FromBody] AdvertisementDto advertisementDTO)
        {
            Log.Information("Requested creating advertisement from IP: {IP} by user: {User}", HttpContext.Connection.RemoteIpAddress, User.Identity.Name);
            var isCreated = await _advertisementService.CreateAdvertisement(advertisementDTO);
            if (isCreated)
            {
                Log.Information("Advertisement {Slogan} created successfully by user: {User} from IP: {IP}", advertisementDTO.Slogan, User.Identity.Name, HttpContext.Connection.RemoteIpAddress);
                return Ok(isCreated);
            }
            Log.Warning("Failed to create advertisement {Slogan} by user: {User} from IP: {IP}", advertisementDTO.Slogan, User.Identity.Name, HttpContext.Connection.RemoteIpAddress);
            return BadRequest("Advertisement could not be created");
        }

        [HttpGet("getAll")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<IEnumerable<AdvertisementDto>>> GetAllAdvertisements()
        {
            Log.Information("Requested all advertisements by user: {User} from IP: {IP}", User.Identity.Name, HttpContext.Connection.RemoteIpAddress);
            var ads = await _advertisementService.GetAllAdvertisements();
            Log.Information("All advertisements returned successfully for user: {User} from IP: {IP}", User.Identity.Name, HttpContext.Connection.RemoteIpAddress);
            return Ok(ads);
        }

        [HttpPost("update")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<bool>> UpdateAdvertisement([FromBody] AdvertisementDto ad)
        {
            Log.Information("Requested update for advertisement {Slogan} by user: {User} from IP: {IP}", ad.Slogan, User.Identity.Name, HttpContext.Connection.RemoteIpAddress);
            var isUpdated = await _advertisementService.UpdateAdvertisement(ad);
            if (isUpdated)
            {
                Log.Information("Advertisement {Slogan} updated successfully by user: {User} from IP: {IP}", ad.Slogan, User.Identity.Name, HttpContext.Connection.RemoteIpAddress);
                return Ok(isUpdated);
            }
            Log.Warning("Failed to update advertisement {Slogan} by user: {User} from IP: {IP}", ad.Slogan, User.Identity.Name, HttpContext.Connection.RemoteIpAddress);
            return NotFound("Advertisement could not be updated");
        }
    }
}
