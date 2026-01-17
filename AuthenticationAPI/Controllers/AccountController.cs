using AuthenticationAPI.DTO;
using AuthenticationAPI.Services;
using AuthenticationAPI.Services.ServiceObjects.AccountServiceObjects;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationAPI.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IAccountService accountService;
        private AccountServiceResponse? response;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAccountAsync(RegisterAccountDto requestDto)
        {
            response = await accountService.RegisterAccountAsync(requestDto);
            return response.GetResult();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAccountAsync(LoginAccountDto requestDto)
        {
            response = await accountService.LoginAccountAsync(requestDto);
            return response.GetResult();
        }
    }
}
