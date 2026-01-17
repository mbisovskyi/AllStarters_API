using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AuthenticationAPI.Data
{
    public class SqlDataInterface : ISqlDataInterface
    {
        private readonly IDbConnection connection;
        public SqlDataInterface(IConfiguration config)
        {
            connection = new SqlConnection(config.GetConnectionString("AuthenticationDatabase") ?? string.Empty);
        }

        public async Task<IEnumerable<Res>> RawQueryGetMany<Res>(string query)
        {
            return await connection.QueryAsync<Res>(query);
        }

        public async Task<IEnumerable<Res>> RawQueryGetMany<Res, P>(string query, P parameters)
        {
            return await connection.QueryAsync<Res>(query, parameters);
        }

        public async Task<Res> RawQueryGetFirstOrDefault<Res, P>(string query, P parameters)
        {
            return await connection.QueryFirstOrDefaultAsync<Res>(query, parameters);
        }
    }
}
