using Marketing_system.BL.Contracts.DTO;
using Marketing_system.DA.Contracts.Model;

namespace Marketing_system.DA.Contracts.IRepository
{
    public interface ITokenGeneratorRepository
    {
        Task<AuthenticationTokensDto> GenerateTokens(User user);
        string CreateRefreshToken();
        Task<bool> ValidateAccessToken(string token);
    }
}
