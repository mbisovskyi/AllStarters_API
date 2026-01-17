using AuthenticationAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationAPI.Services
{
    public interface IUserRolesService
    {
        Task<IdentityResult> AddRoleToUserAsync(User user, string roleName);
        Task<bool> IsUserRoleValidAsync(string userRole);
        Task PopulateRolesTableRowsAsync();
    }
}