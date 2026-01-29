using AuthenticationAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationAPI.Extensions
{
    public static class AuthenticationServiceExtensions
    {
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Validates what application has issued a token. This can be multiple Issuers if needed.
                    ValidateIssuer = true,
                    ValidIssuer = config["AuthenticationJwtSettings:Issuer"] ?? throw new InvalidOperationException("Jwt Issuer is not configured."),

                    // Validates what application token is issued for. This can be multiple Audiences if needed.
                    ValidateAudience = false,
                    ValidAudience = config["AuthenticationJwtSettings:Audience"] ?? throw new InvalidOperationException("Jwt Audience is not configured."),

                    // Validates if token has expired.
                    ValidateLifetime = true,

                    // Validates if token has been issued using configured SecretKey.
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config["AuthenticationJwtSettings:SecretKey"] ?? string.Empty)),
                    ClockSkew = TimeSpan.Zero
                };

                // Validates custom token events.
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        UserManager<User> userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
                        User? user = await userManager.GetUserAsync(context.Principal!);

                        // Check if account exists
                        if (user == null)
                        {
                            context.Fail("Account does not exist.");
                            return;
                        }

                        // Check lockout
                        if (await userManager.IsLockedOutAsync(user))
                        {
                            context.Fail("Account is locked out.");
                            return;
                        }
                    }
                };
            });

            return services;
        }
    }
}
