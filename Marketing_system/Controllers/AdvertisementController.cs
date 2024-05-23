using Marketing_system.BL.Contracts.DTO;
using Marketing_system.BL.Contracts.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketing_system.Controllers;

[Route("api/advertisements")]
public class AdvertisementController : Controller
{
    private readonly IAdvertisementService _advertisementService;
    public AdvertisementController(IAdvertisementService advertisementService)
    {
        _advertisementService = advertisementService;
    }

    [HttpPost("create")]
    //[Authorize(Roles = "Client")]
    public async Task<ActionResult<bool>> CreateAdvertisement([FromBody] AdvertisementDto advertisementDTO)
    {
       var isCreated = await _advertisementService.CreateAdvertisement(advertisementDTO);
        if (isCreated)
            return Ok(isCreated);
        return BadRequest(!isCreated);
    }
    [HttpGet("getAll")]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<IEnumerable<AdvertisementDto>>> GetAllAdvertisements()
    {
        var ads = await _advertisementService.GetAllAdvertisements();
        return Ok(ads);
    }
    [HttpPost("update")]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<bool>> UpdateAdvertisement([FromBody] AdvertisementDto ad)
    {
        var isUpdated = await _advertisementService.UpdateAdvertisement(ad);
        if (isUpdated)
        {
            return Ok(isUpdated);
        }
        return NotFound();
    }

}
