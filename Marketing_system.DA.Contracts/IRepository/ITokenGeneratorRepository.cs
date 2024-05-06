using Marketing_system.BL.Contracts.DTO;
using Marketing_system.DA.Contracts.Model;

namespace Marketing_system.DA.Contracts.IRepository
{
    public interface ITokenGeneratorRepository
    {
        Task<AuthenticationTokensDto> GenerateAccessToken(User user);
    }
}
