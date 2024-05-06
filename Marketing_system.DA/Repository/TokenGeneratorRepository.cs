using Marketing_system.BL.Contracts.DTO;
using Marketing_system.DA.Contracts.IRepository;
using Marketing_system.DA.Contracts.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Marketing_system.DA.Repository
{
    public class TokenGeneratorRepository : ITokenGeneratorRepository
    {
        private readonly string _key = Environment.GetEnvironmentVariable("JWT_KEY") ?? "marketingsystem_secret_key";
        private readonly string _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "marketingsystem";
        private readonly string _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "marketingsystem-front.com";

        public async Task<AuthenticationTokensDto> GenerateAccessToken(User user)
        {
            var authenticationResponse = new AuthenticationTokensDto();
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("id", user.Id.ToString()),
                new("email", user.Email),
                new("userRole", user.Role.ToString()),
            };
            var jwt = CreateToken(claims, 60*24);
            authenticationResponse.Id = user.Id;
            authenticationResponse.AccessToken = jwt;
            return authenticationResponse;
        }
        private string CreateToken(IEnumerable<Claim> claims, double expirationTimeInMinutes)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _issuer,
                _audience,
                claims,
                expires: DateTime.Now.AddMinutes(expirationTimeInMinutes),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
