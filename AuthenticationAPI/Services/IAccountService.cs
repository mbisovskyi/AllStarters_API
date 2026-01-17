using AuthenticationAPI.DTO;
using AuthenticationAPI.Services.ServiceObjects.AccountServiceObjects;

namespace AuthenticationAPI.Services
{
    public interface IAccountService
    {
        Task CreateSuperAccountAsync(RegisterAccountDto requestDto);
        Task<AccountServiceResponse> RegisterAccountAsync(RegisterAccountDto requestDto);
    }
}