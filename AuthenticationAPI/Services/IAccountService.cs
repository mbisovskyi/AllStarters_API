using AuthenticationAPI.DTO;
using AuthenticationAPI.Services.ServiceObjects;
using AuthenticationAPI.Services.ServiceObjects.AccountServiceObjects;

namespace AuthenticationAPI.Services
{
    public interface IAccountService
    {
        Task<RegisterAccountResponse> RegisterAccountAsync(RegisterAccountDto requestDto);
        Task<LoginAccountResponse> LoginAccountAsync(LoginAccountDto requestDto);
    }
}