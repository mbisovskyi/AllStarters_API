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
        private AccountServiceResponse response = new AccountServiceResponse();
        private IdentityResult result = new IdentityResult();

        public AccountService(UserManager<User> userManager, IUserRolesService userRoleService, RoleManager<IdentityRole> roleManager, IJwtTokenService tokenService)
        {
            this.userManager = userManager;
            this.userRoleService = userRoleService;
            this.roleManager = roleManager;
            this.tokenService = tokenService;
        }

        public async Task<AccountServiceResponse> RegisterAccountAsync(RegisterAccountDto requestDto)
        {
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
                    response.SetOk(new
                    {
                        Message = $"{requestDto.UserRole} Account was successfully created.",
                        CreatedDt = user.CreatedDate
                    });
                }

                if (!result.Succeeded)
                {
                    // Make sure that user does not exist if successful response is not generated
                    if (userManager.GetUserNameAsync(user) != null)
                        await userManager.DeleteAsync(user);

                    response.SetBadRequestErrors(result.Errors);
                }

                return response;
            }
            catch (Exception)
            {
                // Make sure that User is not created in case of any exception
                if (userManager.GetUserNameAsync(user) != null)
                    await userManager.DeleteAsync(user);

                response.SetBadRequestErrors("Internal Error.");
                return response;
            }
        }

        public async Task<AccountServiceResponse> LoginAccountAsync(LoginAccountDto requestDto)
        {
            User? user = await userManager.FindByEmailAsync(requestDto.Email);
            if (user != null && await userManager.CheckPasswordAsync(user, requestDto.Password))
            {
                IList<Claim> userClaims = await GetAccountClaimsAsync(user);
                string token = tokenService.CreateToken(userClaims);
                result = await userManager.SetAuthenticationTokenAsync(user, "JwtProvider", Constants.TokenTypes.Login, token);

                if (result.Succeeded)
                {
                    response.SetOk(new { AuthorizationToken = token });
                }
                else
                {
                    response.SetBadRequestErrors(result.Errors);
                }
            }
            else
            {
                response.SetBadRequestErrors("Unauthorized.");
            }

            return response;
        }

        private async Task<IList<Claim>> GetAccountClaimsAsync(User user)
        {
            IList<Claim> userClaims = await userManager.GetClaimsAsync(user);
            IList<string> userRoles = await userManager.GetRolesAsync(user);

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
    }
}
