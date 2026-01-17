
using AuthenticationAPI.Data.TableRowModels;

namespace AuthenticationAPI.Data.DataAccessLayers
{
    public interface IRolesDataAccessLayer
    {
        Task<IEnumerable<ValidUserRoleTableRow>> GetValidUserRoles();
        Task<bool> IsUserRoleValidAsync(string userRole);
    }
}