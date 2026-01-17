using AuthenticationAPI.DTO;
using AuthenticationAPI.Services.ServiceObjects.AccountServiceObjects;

namespace AuthenticationAPI.Services
{
    public interface IAccountService
    {
        Task<AccountServiceResponse> RegisterAccountAsync(RegisterAccountDto requestDto);
        Task<AccountServiceResponse> LoginAccountAsync(LoginAccountDto requestDto);
    }
}