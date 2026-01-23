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
        public async Task<ActionResult<RegisterAccountResponse>> RegisterAccountAsync(RegisterAccountDto requestDto)
        {
            RegisterAccountResponse response =  await accountService.RegisterAccountAsync(requestDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginAccountResponse>> LoginAccountAsync(LoginAccountDto requestDto)
        {
            LoginAccountResponse response = await accountService.LoginAccountAsync(requestDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
