namespace AuthenticationAPI.Extensions
{
    public static class AuthorizationServicesExtensions
    {
        public static IServiceCollection AddAuthorizationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthorization(options =>
            {
                // AS AN EXAMPLE FOR NOW! Make sure you need this authorization policy configured!
                options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin", "true"));
            });

            return services;
        }
    }
}
