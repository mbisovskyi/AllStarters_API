using Microsoft.AspNetCore.Mvc;

namespace AuthenticationAPI.Services.ServiceObjects.AccountServiceObjects
{
    public class RegisterAccountResponse : BaseServiceResponse
    {
        public DateTime CreatedDt;
    }

    public class LoginAccountResponse : BaseServiceResponse
    {
        public string AccessToken { get; set; } = string.Empty;
    }

    public class GetMeAccountResponse : BaseServiceResponse
    {
        public string UserName { get; set; } = string.Empty;
        public List<string> UserRoles { get; set; } = new List<string>();
    }
}
