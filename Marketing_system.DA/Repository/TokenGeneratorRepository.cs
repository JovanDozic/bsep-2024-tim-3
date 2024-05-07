using Marketing_system.BL.Contracts.DTO;
using Marketing_system.DA.Contracts.IRepository;
using Marketing_system.DA.Contracts.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Marketing_system.DA.Repository
{
    public class TokenGeneratorRepository : ITokenGeneratorRepository
    {
        private readonly string _key = Environment.GetEnvironmentVariable("JWT_KEY") ?? "marketingsystem_superssecret_key";
        private readonly string _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "marketingsystem";
        private readonly string _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "marketingsystem-front.com";

        public async Task<AuthenticationTokensDto> GenerateTokens(User user)
        {
            var authenticationResponse = new AuthenticationTokensDto();
            try
            {
                var claims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new("id", user.Id.ToString()),
                    new("email", user.Email),
                    new("userRole", user.Role.ToString()),
                    new(ClaimTypes.Role, user.GetPrimaryRoleName())
                };

                var accessClaims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new("id", user.Id.ToString()),
                    new(ClaimTypes.Role, user.GetPrimaryRoleName())
                };
                var jwt = CreateAccessToken(claims, 60 * 24);
                var refToken = CreateAccessToken(accessClaims, 15);
                authenticationResponse.Id = user.Id;
                authenticationResponse.AccessToken = jwt;
                authenticationResponse.RefreshToken = refToken;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while generating tokens: {ex.Message}");
                // Rethrow the exception to propagate it further
                throw;
            }
            return authenticationResponse;
        }

        public string CreateAccessToken(IEnumerable<Claim> claims, double expirationTimeInMinutes)
        {
            if (_key.Length < 32)
            {
                throw new InvalidOperationException("Key size must be at least 32 bytes for HMAC-SHA256 algorithm.");
            }

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

        public async Task<bool> ValidateAccessToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true
            };

            try
            {
                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
                return true;
            }
            catch (SecurityTokenException)
            {
                return false;
            }
        }

        private string GenerateValidKey()
        {
            var keyLength = 32;

            var keyBytes = new byte[keyLength];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(keyBytes);
            }

            return Convert.ToBase64String(keyBytes);
        }


    }
}
