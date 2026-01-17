using AuthenticationAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationAPI.Extensions
{
    public static class DbContextServicesExtensions
    {
        public static IServiceCollection AddDbContextServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("AuthenticationDatabase"));
            });

            return services;
        }
    }
}
