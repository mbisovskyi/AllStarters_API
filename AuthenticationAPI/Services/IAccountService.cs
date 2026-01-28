using AuthenticationAPI.DTO;
using AuthenticationAPI.Services.ServiceObjects;
using AuthenticationAPI.Services.ServiceObjects.AccountServiceObjects;
using System.Security.Claims;

namespace AuthenticationAPI.Services
{
    public interface IAccountService
    {
        Task<RegisterAccountResponse> RegisterAccountAsync(RegisterAccountDto requestDto);
        Task<LoginAccountResponse> LoginAccountAsync(LoginAccountDto requestDto);
        Task<AuthenticateAccountResponse> AuthenticateAccountAsync(ClaimsPrincipal principal);
        Task<bool> VerifyAccessAsync(ClaimsPrincipal principal);
    }
}