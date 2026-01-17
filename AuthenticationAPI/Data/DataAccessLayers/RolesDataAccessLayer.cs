using AuthenticationAPI.Data.TableRowModels;

namespace AuthenticationAPI.Data.DataAccessLayers
{
    public class RolesDataAccessLayer : IRolesDataAccessLayer
    {
        private readonly ISqlDataInterface sqlDataInterface;

        public RolesDataAccessLayer(ISqlDataInterface sqlDataInterface)
        {
            this.sqlDataInterface = sqlDataInterface;
        }

        public async Task<IEnumerable<ValidUserRoleTableRow>> GetValidUserRoles()
        {
            string tableName = "ValidUserRoles";
            string query = $"SELECT * FROM dbo.{tableName}";
            return await sqlDataInterface.RawQueryGetMany<ValidUserRoleTableRow>(query);
        }

        public async Task<bool> IsUserRoleValidAsync(string userRole)
        {
            string tableName = "Roles";
            string query = $"SELECT 1 FROM dbo.{tableName} WHERE Name = @Role"; 
            object parameters = new { Role = userRole };
            return await sqlDataInterface.RawQueryGetFirstOrDefault<bool, object>(query, parameters); // If any record is returned then it will be transcripted to True. 
        }
    }
}
