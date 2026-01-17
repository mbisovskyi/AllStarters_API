using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace AuthenticationAPI.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private byte[] secretKey;
        private int expiresInMinutes;

        public JwtTokenService(IConfiguration config)
        {
            GetTokenConfiguration(config);
        }

        public string CreateToken(IList<Claim> claims)
        {
            var claimsDict = new Dictionary<string, object>();
            if (claims != null && claims.Count > 0)
            {
                foreach (var claim in claims)
                {
                    claimsDict.Add(claim.Type, claim.Value);
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature),
                Claims = claimsDict,
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                NotBefore = DateTime.UtcNow
            };

            var tokenHandler = new JsonWebTokenHandler();
            return tokenHandler.CreateToken(tokenDescriptor);
        }

        private void GetTokenConfiguration(IConfiguration config)
        {
            secretKey = System.Text.Encoding.UTF8.GetBytes(config["AuthenticationJwtSettings:SecretKey"] ?? String.Empty);
            if (!Int32.TryParse(config[""], out expiresInMinutes))
            {
                expiresInMinutes = 5;
            }
        }
    }
}
