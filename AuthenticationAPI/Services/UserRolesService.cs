using AuthenticationAPI.Data.DataAccessLayers;
using AuthenticationAPI.Data.TableRowModels;
using AuthenticationAPI.Models;
using AuthenticationAPI.SystemObjects;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AuthenticationAPI.Services
{
    public class UserRolesService : IUserRolesService
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<User> userManager;
        private readonly IRolesDataAccessLayer rolesDAL;

        private IdentityRole? role = new IdentityRole();

        public UserRolesService(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IRolesDataAccessLayer rolesDAL)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.rolesDAL = rolesDAL;
        }
        public async Task<IdentityResult> AddRoleToUserAsync(User user, string roleName)
        {
            // Initialize failed result
            IdentityResult result = IdentityResult.Failed();

            // Process if role is valid
            if (await roleManager.RoleExistsAsync(roleName))
            {
                role = await roleManager.FindByNameAsync(roleName);

                if (role != null)
                {
                    // Add role to user if user is not yet in the role
                    if (!await userManager.IsInRoleAsync(user, role.Name!))
                    {
                        result = await userManager.AddToRoleAsync(user, role.Name!);

                        // Add role claims
                        if (result.Succeeded)
                            result = await AddRoleClaims();
                    }
                }
            }

            return result;
        }

        public async Task<bool> IsUserRoleValidAsync(string userRole)
        {
            return await rolesDAL.IsUserRoleValidAsync(userRole);
        }

        private async Task<IdentityResult> AddRoleClaims()
        {
            // Initialize success result
            IdentityResult result = IdentityResult.Failed();
            List<Claim> claims = new List<Claim>();

            // Define route base on Role Name
            switch (role?.Name ?? string.Empty)
            {
                case Constants.UserRoles.SuperRole:
                    claims = CompileSuperRoleClaims();
                    break;
                case Constants.UserRoles.AdminRole:
                    claims = CompileAdminRoleClaims();
                    break;
                case Constants.UserRoles.UserRole:
                    claims = CompileUserRoleClaims();
                    break;
            }

            return await CreateClaims(claims);
        }

        private async Task<IdentityResult> CreateClaims(List<Claim> claims)
        {
            if (role != null)
            {
                foreach (Claim claim in claims)
                {
                    if (!await RoleClaimExists(role, claim))
                        return await roleManager.AddClaimAsync(role, claim);

                    // Role Claim already exists. No need to add a new one. It is success.
                    return IdentityResult.Success;
                }
            }
            return IdentityResult.Failed();
        }

        private List<Claim> CompileSuperRoleClaims()
        {
            return new List<Claim>
            {
                new Claim(Constants.UserRoles.SuperRole, true.ToString())
            };
        }

        private List<Claim> CompileAdminRoleClaims()
        {
            return new List<Claim>
            {
                new Claim(Constants.UserRoles.AdminRole, true.ToString())
            };
        }

        private List<Claim> CompileUserRoleClaims()
        {
            return new List<Claim>
            {
                new Claim(Constants.UserRoles.UserRole, true.ToString())
            };
        }

        private async Task<bool> RoleClaimExists(IdentityRole role, Claim claim)
        {
            var roleClaims = await roleManager.GetClaimsAsync(role);
            if (!roleClaims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// This will populate Asp.Net Identity Roles table with records using ValidUserRoles configuration table
        /// </summary>
        public async Task PopulateRolesTableRowsAsync()
        {
            IEnumerable<ValidUserRoleTableRow> validRoleTableRows = rolesDAL.GetValidUserRoles().Result;
            foreach (ValidUserRoleTableRow validRoleTableRow in validRoleTableRows)
            {
                if (!await roleManager.RoleExistsAsync(validRoleTableRow.RoleName))
                    await roleManager.CreateAsync(new IdentityRole(validRoleTableRow.RoleName));
            }
        }
    }
}
