using Marketing_system.DA.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace Marketing_system
{
    public class AuthenticationMiddleware
    {
        private readonly IServiceProvider _todoUserService;
        private readonly RequestDelegate _next;
        private readonly IUnitOfWork _unitOfWork;

        public AuthenticationMiddleware(RequestDelegate next, IServiceProvider todoUserService)
        {
            _next = next;
            _todoUserService = todoUserService;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("api/users/login"))
            {
                await _next(context);
                return;
            }
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
            var jwtHandler = new JwtSecurityTokenHandler();
            var tokenS = jwtHandler.ReadToken(token) as JwtSecurityToken;
            var userId = tokenS.Claims.First(claim => claim.Type == "id").Value;

            var isValid = await _unitOfWork.GetTokenGeneratorRepository().ValidateAccessToken(token);
            if (!isValid)
            {
                var user = await _unitOfWork.GetUserRepository().GetByIdAsync(Convert.ToInt32(userId));
                var accessToken = jwtHandler.ReadToken(user.RefreshToken) as JwtSecurityToken;
                var expirationDate = accessToken.ValidTo;
                if(expirationDate < DateTime.UtcNow)
                {
                    var tokens = await _unitOfWork.GetTokenGeneratorRepository().GenerateTokens(user);
                    user.RefreshToken = tokens.RefreshToken;
                    await _unitOfWork.Save();
                    context.Response.Headers.Add("Authorization", "Bearer " + tokens.AccessToken);
                    await _next(context);
                }
                else
                {
                    var accessClaims = new List<Claim>
                    {
                        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new("id", user.Id.ToString()),
                        new(ClaimTypes.Role, user.GetPrimaryRoleName())
                    };
                    var newAccessToken = _unitOfWork.GetTokenGeneratorRepository().CreateAccessToken(accessClaims, 15);
                    context.Response.Headers.Add("Authorization", "Bearer " + newAccessToken);
                    await _next(context);
                }
            }
            else
            {
                context.Response.StatusCode = 200;
                await _next(context);
            }
        }
    }
}