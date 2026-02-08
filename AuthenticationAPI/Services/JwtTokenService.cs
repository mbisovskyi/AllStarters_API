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
        private int refreshTokenExpiresInMinutes;
        private string tokenIssuer;
        private string tokenAudience;

        public JwtTokenService(IConfiguration config, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            GetTokenConfiguration(config);
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.context = context;
        }

        public async Task<string> CreateTokenAsync(User user, bool lifetimeExtended, string tokenType)
        {
            SecurityTokenDescriptor tokenDescriptor = GetTokenDescriptor();

            IList<Claim> userClaims = await GetUserClaimsAsync(user);
            if (userClaims != null && userClaims.Count > 0)
            {
                foreach (var claim in userClaims)
                {
                    tokenDescriptor.Claims.Add(claim.Type, claim.Value);
                }
            }

            switch (tokenType)
            {
                case Constants.TokenTypes.Login:
                    tokenDescriptor.Expires = DateTime.UtcNow.AddMinutes(lifetimeExtended ? expiresInMinutesExtended : expiresInMinutes);
                    break;
                case Constants.TokenTypes.Refresh:
                    tokenDescriptor.Expires = DateTime.UtcNow.AddMinutes(refreshTokenExpiresInMinutes);
                    break;
                default:
                    throw new InvalidOperationException("Invalid token type.");
            }

            var tokenHandler = new JsonWebTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            bool success = await UpdateUserTokenAsync(user, Constants.TokenProviders.Jwt, tokenType, token, lifetimeExtended);

            if (success)
                return token;

            return null;
        }

        private SecurityTokenDescriptor GetTokenDescriptor()
        {
            return new SecurityTokenDescriptor
            {
                Issuer = tokenIssuer,
                Audience = tokenAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature),
                NotBefore = DateTime.UtcNow,
                Claims = new Dictionary<string, object>()
            };
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
                expiresInMinutes = 5; // Default value for normal token expiration is 5 minutes.
            }
            if (!Int32.TryParse(config["AuthenticationJwtSettings:TokenExtendedExpirationMinutes"], out expiresInMinutesExtended))
            {
                expiresInMinutesExtended = expiresInMinutes; // Default value for extended token expiration is the same as normal token expiration.
            }
            if (!Int32.TryParse(config["AuthenticationJwtSettings:RefreshTokenExpirationMinutes"], out refreshTokenExpiresInMinutes))
            {
                expiresInMinutesExtended = expiresInMinutes * 2; // Default value for refresh token expiration is 2 times the normal token expiration.
            }
            
        }

        private async Task<bool> UpdateUserTokenAsync(User user, string loginProvider, string tokenType, string token, bool lifetimeExtended)
        {
            var userTokenRow = await context.Set<UserTokenTableRow>()
                .FirstOrDefaultAsync(row =>
                    row.UserId == user.Id &&
                    row.LoginProvider == loginProvider &&
                    row.Name == tokenType);

            if (userTokenRow == null)
            {
                userTokenRow = new UserTokenTableRow
                {
                    UserId = user.Id,
                    LoginProvider = loginProvider,
                    Name = tokenType,
                    Value = token,
                    LifetimeExtended = lifetimeExtended
                };
                context.Set<UserTokenTableRow>().Add(userTokenRow);
            } 
            else
            {
                userTokenRow.Value = token;
                userTokenRow.LifetimeExtended = lifetimeExtended;
            }

            await context.SaveChangesAsync();

            return true;
        }
    }
}
