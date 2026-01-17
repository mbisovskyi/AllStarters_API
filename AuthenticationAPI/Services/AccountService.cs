using AuthenticationAPI.DTO;
using AuthenticationAPI.Models;
using AuthenticationAPI.Services.ServiceObjects.AccountServiceObjects;
using AuthenticationAPI.SystemObjects;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> userManager;
        private readonly IUserRolesService userRoleService;

        public AccountService(UserManager<User> userManager, IUserRolesService userRoleService)
        {
            this.userManager = userManager;
            this.userRoleService = userRoleService;
        }

        public async Task<AccountServiceResponse> RegisterAccountAsync(RegisterAccountDto requestDto)
        {
            AccountServiceResponse response = new AccountServiceResponse();

            // Initialize default BadRequest response result object
            response.SetBadRequestResult($"{requestDto.UserRole} Account was not created!");

            User user = new User
            {
                UserName = requestDto.UserName,
                Email = requestDto.Email,
                CreatedDate = DateTime.Now
            };

            try
            {
                // Create Account
                IdentityResult result = await userManager.CreateAsync(user, requestDto.Password);

                // Add requested User Role if account was successfully created
                if (result.Succeeded)
                    result = await userRoleService.AddRoleToUserAsync(user, requestDto.UserRole);

                // Generate successful response that account was generated with requested role
                if (result.Succeeded)
                {
                    response.SetOkResult(new
                    {
                        Message = $"{requestDto.UserRole} Account was successfully created.",
                        CreatedDt = user.CreatedDate
                    });
                }
                else
                {
                    // Make sure that user does not exist if successful response is not generated
                    if (userManager.GetUserNameAsync(user) != null)
                        await userManager.DeleteAsync(user);
                }

                return response;
            }
            catch (Exception)
            {
                // Make sure that User is not created in case of any exception
                if (userManager.GetUserNameAsync(user) != null)
                    await userManager.DeleteAsync(user);

                return response;
            }
        }

        public async Task CreateSuperAccountAsync(RegisterAccountDto requestDto)
        {
            IList<User> superUsersList = await userManager.GetUsersInRoleAsync(Constants.UserRoles.SuperRole);

            // At least one super account should exist
            if (superUsersList.Count <= 0)
            {

            }
        }
    }
}
