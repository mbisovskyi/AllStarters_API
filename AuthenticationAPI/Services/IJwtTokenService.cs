using System.Security.Claims;

namespace AuthenticationAPI.Services
{
    public interface IJwtTokenService
    {
        string CreateToken(IList<Claim> claims);
    }
}