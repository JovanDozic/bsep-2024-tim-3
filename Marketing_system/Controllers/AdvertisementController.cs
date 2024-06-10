using System.Reflection;
using Marketing_system.BL.Contracts.DTO;
using Marketing_system.BL.Contracts.IService;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Microsoft.AspNetCore.RateLimiting;

namespace Marketing_system.Controllers;

[Route("api/advertisements")]
public class AdvertisementController : Controller
{
    private readonly IAdvertisementService _advertisementService;

    public AdvertisementController(IAdvertisementService advertisementService)
    {
        _advertisementService = advertisementService;
    }
    [HttpGet("basic")]
    [EnableRateLimiting("fixed-basic")]
    public async Task<ActionResult<bool>> AdClickSimulationBasic()
    {
        return Ok(true);
    }
    [HttpGet("standard")]
    [EnableRateLimiting("fixed-standard")]
    public async Task<ActionResult<bool>> AdClickSimulationStandard()
    {
        return Ok(true);
    }
    [HttpGet("golden")]
    [EnableRateLimiting("fixed-golden")]
    public async Task<ActionResult<bool>> AdClickSimulationGold()
    {
        return Ok(true);
    }

        [HttpPost("create")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<bool>> CreateAdvertisement([FromBody] AdvertisementDto advertisementDTO)
        {
            Log.Information($"Requested creating advertisement from IP: {HttpContext.Connection.RemoteIpAddress} by user: {User.Identity.Name}");
            var isCreated = await _advertisementService.CreateAdvertisement(advertisementDTO);
            if (isCreated)
            {
                Log.Information($"Advertisement {advertisementDTO.Slogan} created successfully by user: {User.Identity.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
                return Ok(isCreated);
            }
            Log.Warning($"Failed to create advertisement {advertisementDTO.Slogan} by user: {User.Identity.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
            return BadRequest("Advertisement could not be created");
        }

        [HttpGet("getAll")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<IEnumerable<AdvertisementDto>>> GetAllAdvertisements()
        {
            Log.Information($"Requested all advertisements by user: {User.Identity.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
            var ads = await _advertisementService.GetAllAdvertisements();
            Log.Information($"All advertisements returned successfully for user: {User.Identity.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
            return Ok(ads);
        }

        [HttpPost("update")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<bool>> UpdateAdvertisement([FromBody] AdvertisementDto ad)
        {
            Log.Information($"Requested update for advertisement {ad.Slogan} by user: {User.Identity.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
            var isUpdated = await _advertisementService.UpdateAdvertisement(ad);
            if (isUpdated)
            {
                Log.Information($"Advertisement {ad.Slogan} updated successfully by user: {User.Identity.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
                return Ok(isUpdated);
            }
            Log.Warning($"Failed to update advertisement {ad.Slogan} by user: {User.Identity.Name} from IP: {HttpContext.Connection.RemoteIpAddress}");
            return NotFound("Advertisement could not be updated");
        }
    }
}
