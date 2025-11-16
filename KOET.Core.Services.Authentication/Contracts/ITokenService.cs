using KOET.Core.Services.Authentication.Models;
using System.Security.Claims;

namespace KOET.Core.Services.Authentication.Contracts
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }

}
