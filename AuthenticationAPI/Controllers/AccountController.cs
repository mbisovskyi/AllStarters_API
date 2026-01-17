using AuthenticationAPI.DTO;
using AuthenticationAPI.Services;
using AuthenticationAPI.Services.ServiceObjects.AccountServiceObjects;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationAPI.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAccountAsync(RegisterAccountDto requestDto)
        {
            AccountServiceResponse response = await accountService.RegisterAccountAsync(requestDto);
            return response.GetResult();
        }

    }
}
