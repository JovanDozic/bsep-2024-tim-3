using Marketing_system.BL.Contracts.DTO;
using Marketing_system.DA.Contracts.Model;
using System.Security.Claims;

namespace Marketing_system.DA.Contracts.IRepository
{
    public interface ITokenGeneratorRepository
    {
        Task<TokensDto> GenerateTokens(User user);
        Task<bool> ValidateAccessToken(string token);
        string CreateAccessToken(IEnumerable<Claim> claims, double expirationTimeInMinutes);
        string GenerateTempToken(string username);
    }
}
