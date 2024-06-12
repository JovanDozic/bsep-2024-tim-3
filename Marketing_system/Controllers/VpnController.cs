using Microsoft.AspNetCore.Mvc;

namespace Marketing_system.Controllers;

[Route("api/vpn")]
public class VpnController : Controller
{
    private readonly HttpClient _httpClient;

    public VpnController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    [HttpGet]
    public async Task<ActionResult<string>> GetFromHiddenComponent()
    {
        try
        {
            var response = await _httpClient.GetAsync("http://10.13.13.1:3000/");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
