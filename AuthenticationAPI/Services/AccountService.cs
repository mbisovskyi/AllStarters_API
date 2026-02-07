using AuthenticationAPI.DTO;
using AuthenticationAPI.Models;
using AuthenticationAPI.Services.ServiceObjects.AccountServiceObjects;
using AuthenticationAPI.SystemObjects;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AuthenticationAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> userManager;
        private readonly IUserRolesService userRoleService;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IJwtTokenService tokenService;
        private IdentityResult result = new IdentityResult();

        public AccountService(UserManager<User> userManager, IUserRolesService userRoleService, RoleManager<IdentityRole> roleManager, IJwtTokenService tokenService)
        {
            this.userManager = userManager;
            this.userRoleService = userRoleService;
            this.roleManager = roleManager;
            this.tokenService = tokenService;
        }

        public async Task<RegisterAccountResponse> RegisterAccountAsync(RegisterAccountDto requestDto)
        {
            RegisterAccountResponse response = new RegisterAccountResponse();

            User user = new User
            {
                UserName = requestDto.UserName,
                Email = requestDto.Email,
                CreatedDate = DateTime.Now
            };

            try
            {
                // Create Account
                result = await userManager.CreateAsync(user, requestDto.Password);

                // Add requested User Role if account was successfully created
                if (result.Succeeded)
                    result = await userRoleService.AddRoleToUserAsync(user, requestDto.UserRole);

                // Generate successful response that account was generated with requested role
                if (result.Succeeded)
                {
                    response.Success = true;
                    response.CreatedDt = DateTime.Now;
                }

                if (!result.Succeeded)
                {
                    // Make sure that user does not exist if successful response is not generated
                    if (userManager.GetUserNameAsync(user) != null)
                    {
                        await userManager.DeleteAsync(user);
                        response.SetErrors(result.Errors);
                    }
                }

                return response;
            }
            catch (Exception)
            {
                // Make sure that User is not created in case of any exception
                if (userManager.GetUserNameAsync(user) != null)
                {
                    await userManager.DeleteAsync(user);
                    response.SetErrors("Internal Error.");
                }

                return response;
            }
        }

        public async Task<LoginAccountResponse> LoginAccountAsync(LoginAccountDto requestDto)
        {
            LoginAccountResponse response = new LoginAccountResponse();

            User? user = await userManager.FindByEmailAsync(requestDto.Email);
            if (user != null && await userManager.CheckPasswordAsync(user, requestDto.Password))
            {
                string accessToken = await tokenService.CreateTokenAsync(user, requestDto.RememberMe, Constants.TokenTypes.Login);
                string refreshToken = await tokenService.CreateTokenAsync(user, false, Constants.TokenTypes.Refresh);

                if (accessToken != null && refreshToken != null)
                {
                    response.Success = true;
                    response.AccessToken = accessToken;
                    response.RefreshToken = refreshToken;
                }
            }

            if (!response.Success)
                response.SetErrors("Unauthorized.");

            return response;
        }

        public async Task<GetMeAccountResponse> GetMeAccountAsync(ClaimsPrincipal principal)
        {
            GetMeAccountResponse response = new GetMeAccountResponse();

            try
            {
                User? user = await userManager.GetUserAsync(principal);
                if (user != null)
                {
                    IList<string> userRoles = await userManager.GetRolesAsync(user);

                    response.Success = true;
                    response.UserName = user.UserName!;
                    response.UserRoles = userRoles.ToList();
                }
            }
            catch (Exception)
            {
                response.SetErrors("Internal Error.");
            }

            return response;
        }
    }
}
