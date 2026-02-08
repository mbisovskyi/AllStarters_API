using AuthenticationAPI.Models;

namespace AuthenticationAPI.Services
{
    public interface IJwtTokenService
    {
        Task<string> CreateTokenAsync(User user, bool lifetimeExtended, string tokenType);
    }
}