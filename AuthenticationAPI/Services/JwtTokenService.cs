using AuthenticationAPI.Data;
using AuthenticationAPI.Data.TableModels;
using AuthenticationAPI.Models;
using AuthenticationAPI.SystemObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace AuthenticationAPI.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext context;
        private byte[] secretKey;
        private int expiresInMinutes;
        private int expiresInMinutesExtended;
        private string tokenIssuer;
        private string tokenAudience;

        public JwtTokenService(IConfiguration config, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            GetTokenConfiguration(config);
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.context = context;
        }

        public async Task<string> CreateTokenAsync(User user, bool lifetimeExtended)
        {
            IList<Claim> userClaims = await GetUserClaimsAsync(user);
            var claimsDict = new Dictionary<string, object>();
            if (userClaims != null && userClaims.Count > 0)
            {
                foreach (var claim in userClaims)
                {
                    claimsDict.Add(claim.Type, claim.Value);
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = tokenIssuer,
                Audience = tokenAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature),
                Claims = claimsDict,
                Expires = DateTime.UtcNow.AddMinutes(lifetimeExtended ? expiresInMinutesExtended : expiresInMinutes),
                NotBefore = DateTime.UtcNow
            };

            var tokenHandler = new JsonWebTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            bool result = await UpdateUserTokenAsync(user, Constants.TokenProviders.Jwt, Constants.TokenTypes.Login, token, lifetimeExtended);

            if (result)
                return token;

            return null;
        }

        private async Task<IList<Claim>> GetUserClaimsAsync(User user)
        {
            IList<Claim> userClaims = await userManager.GetClaimsAsync(user);
            IList<string> userRoles = await userManager.GetRolesAsync(user);

            // This is important Claim. This allows to use UserManager.GetUserAsync(ClaimsPrincipal) to find a user.
            userClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));

            foreach (string roleName in userRoles)
            {
                IdentityRole? role = await roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    foreach (Claim claim in await roleManager.GetClaimsAsync(role))
                    {
                        userClaims.Add(claim);
                    }
                }
            }

            return userClaims;
        }

        private void GetTokenConfiguration(IConfiguration config)
        {
            secretKey = System.Text.Encoding.UTF8.GetBytes(config["AuthenticationJwtSettings:SecretKey"] ?? String.Empty);
            tokenIssuer = config["AuthenticationJwtSettings:Issuer"] ?? throw new InvalidOperationException("Jwt Issuer is not configured.");
            tokenAudience = config["AuthenticationJwtSettings:Audience"] ?? throw new InvalidOperationException("Jwt Audience is not configured.");
            if (!Int32.TryParse(config["AuthenticationJwtSettings:TokenExpirationMinutes"], out expiresInMinutes))
            {
                expiresInMinutes = 5;
            }
            if (!Int32.TryParse(config["AuthenticationJwtSettings:TokenExtendedExpirationMinutes"], out expiresInMinutesExtended))
            {
                expiresInMinutesExtended = 5;
            }
        }

        private async Task<bool> UpdateUserTokenAsync(User user, string loginProvider, string tokenName, string token, bool lifetimeExtended)
        {
            IdentityResult result = await userManager.SetAuthenticationTokenAsync(user, loginProvider, tokenName, token);

            if (!result.Succeeded)
                return false;

            var userTokenRow = await context.Set<UserTokenTableRow>()
                .FirstOrDefaultAsync(row =>
                    row.UserId == user.Id &&
                    row.LoginProvider == loginProvider &&
                    row.Name == tokenName);

            if (userTokenRow == null)
                return false;

            userTokenRow.LifetimeExtended = lifetimeExtended;
            await context.SaveChangesAsync();

            return true;
        }
    }
}
