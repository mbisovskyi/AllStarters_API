
namespace AuthenticationAPI.Data
{
    public interface ISqlDataInterface
    {
        Task<IEnumerable<Res>> RawQueryGetMany<Res>(string query);
        Task<IEnumerable<Res>> RawQueryGetMany<Res, P>(string query, P parameters);
        Task<Res> RawQueryGetFirstOrDefault<Res, P>(string query, P parameters);
    }
}