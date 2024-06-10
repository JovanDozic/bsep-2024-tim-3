using Marketing_system.BL.Contracts.IService;
using Microsoft.Extensions.Configuration;

namespace Marketing_system.BL.Service
{
    public class ReCAPTCHAService : IReCAPTCHAService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public ReCAPTCHAService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<bool> VerifyToken(string token)
        {
            return true; // TODO: remove this

            //var secretKey = _configuration["ReCAPTCHA:SecretKey"];
            //var response = await _httpClient.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}", null);
            //var jsonString = await response.Content.ReadAsStringAsync();
            //var jsonResponse = JsonSerializer.Deserialize<ReCAPTCHAResponse>(jsonString);
            //return jsonResponse.Success;
        }
    }
}
