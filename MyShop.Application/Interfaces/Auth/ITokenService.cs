using MyShop.Domain.Identity;
using System.Security.Claims;

namespace MyShop.Application.Interfaces.Auth
{
    public interface ITokenService
    {
        string GenerateAccessToken(ApplicationUser user, IList<string> roles);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
