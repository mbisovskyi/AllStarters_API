using AuthenticationAPI.Data;
using AuthenticationAPI.Data.DataAccessLayers;
using AuthenticationAPI.Services;

namespace AuthenticationAPI.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Data Access Layers
            services.AddScoped<ISqlDataInterface, SqlDataInterface>();
            services.AddScoped<IRolesDataAccessLayer, RolesDataAccessLayer>();

            // Application Services
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserRolesService, UserRolesService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // CORS Policies
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(config["WebApplicationUrl"] ?? string.Empty).AllowAnyHeader().AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
