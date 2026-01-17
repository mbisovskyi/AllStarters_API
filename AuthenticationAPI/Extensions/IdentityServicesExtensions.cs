using AuthenticationAPI.Data;
using AuthenticationAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationAPI.Extensions
{
    public static class IdentityServicesExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentity<User, IdentityRole>(options => 
            {
                IConfigurationSection identitySettings = config.GetSection("IdentitySettings");

                // If PasswordRequiredLength value is not specified in the configuration then it is defaulted to 6 by ASP.NET Core Identity
                if (identitySettings.GetSection("PasswordRequiredLength").Value != null) { options.Password.RequiredLength = Int32.Parse(identitySettings.GetSection("PasswordRequiredLength").Value!); }

                // If PasswordRequireUppercase value is not specified in the configuration then it is defaulted to True by ASP.NET Core Identity
                if (identitySettings.GetSection("PasswordRequireUppercase").Value != null) { options.Password.RequireUppercase = Boolean.Parse(identitySettings.GetSection("PasswordRequireUppercase").Value!); }

                // If PasswordRequireLowercase value is not specified in the configuration then it is defaulted to True by ASP.NET Core Identity
                if (identitySettings.GetSection("PasswordRequireLowercase").Value != null) { options.Password.RequireLowercase = Boolean.Parse(identitySettings.GetSection("PasswordRequireLowercase").Value!); }

                // If MaxFailedAccessAttempts value is not specified in the configuration then it is defaulted to 5 by ASP.NET Core Identity
                if (identitySettings.GetSection("MaxFailedAccessAttempts").Value != null) { options.Lockout.MaxFailedAccessAttempts = Int32.Parse(identitySettings.GetSection("MaxFailedAccessAttempts").Value!); }

                // If DefaultLockoutTimeSpanMinutes value is not specified in the configuration then it is defaulted to 5 minutes by ASP.NET Core Identity
                if (identitySettings.GetSection("DefaultLockoutTimeSpanMinutes").Value != null) { options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(Int32.Parse(identitySettings.GetSection("DefaultLockoutTimeSpanMinutes").Value!)); }

                // If RequireUniqueEmail value is not specified in the configuration then it is defaulted to False minutes by ASP.NET Core Identity
                if (identitySettings.GetSection("RequireUniqueEmail").Value != null) { options.User.RequireUniqueEmail = Boolean.Parse(identitySettings.GetSection("RequireUniqueEmail").Value!); }

            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }
    }
}
