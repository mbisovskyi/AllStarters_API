using AuthenticationAPI.DTO;
using AuthenticationAPI.Models;
using AuthenticationAPI.Services;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationAPI.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void Prepare(this WebApplication app, IConfiguration config)
        {
            var scope = app.Services.CreateScope();
            IServiceProvider serviceProvider = scope.ServiceProvider;

            PrepareUserRoles(serviceProvider).Wait();
            PrepareAccounts(serviceProvider, config).Wait();
        }
        private static async Task PrepareUserRoles(IServiceProvider serviceProvider)
        {
            IUserRolesService userRolesService = serviceProvider.GetRequiredService<IUserRolesService>();
            if (userRolesService != null) await userRolesService.PopulateRolesTableRowsAsync();
        }

        private static async Task PrepareAccounts(IServiceProvider serviceProvider, IConfiguration config)
        {
            RegisterAccountDto requestDto = new RegisterAccountDto();
            IConfigurationSection superAccountSettingsSection = config.GetSection("SuperAccountSettings");
            bool createSuperAccount = false;

            // Check if application settings configured to try to create a super account on application build
            if (superAccountSettingsSection.GetSection("CreateOnBuild").Value != null) { createSuperAccount = Boolean.Parse(superAccountSettingsSection.GetSection("CreateOnBuild").Value!); }
            if (createSuperAccount)
            {
                // Capture super account UserName value from application settings
                if (superAccountSettingsSection.GetSection("UserName").Value != null) { requestDto.UserName = superAccountSettingsSection.GetSection("UserName").Value!; }
                
                // Initialize UserManager service and check if account with this UserName does not exist
                UserManager<User> userManager = serviceProvider.GetRequiredService<UserManager<User>>();
                if (await userManager.FindByNameAsync(requestDto.UserName) == null)
                {
                    // Capture the rest of super account values from application settings
                    if (superAccountSettingsSection.GetSection("Email").Value != null) { requestDto.Email = superAccountSettingsSection.GetSection("Email").Value!; }
                    if (superAccountSettingsSection.GetSection("Password").Value != null) { requestDto.Password = superAccountSettingsSection.GetSection("Password").Value!; }
                    if (superAccountSettingsSection.GetSection("UserRole").Value != null) { requestDto.UserRole = superAccountSettingsSection.GetSection("UserRole").Value!; }

                    // Initialize AccountService to be able to create account
                    IAccountService accountService = serviceProvider.GetRequiredService<IAccountService>();
                    if (accountService != null)
                    {
                        // Create super account
                        await accountService.RegisterAccountAsync(requestDto);
                    }
                }
            }
        }

    }
}
