using AuthenticationAPI.DTO;
using AuthenticationAPI.Services;
using AuthenticationAPI.Services.ServiceObjects.AccountServiceObjects;
using Microsoft.AspNetCore.Authorization;
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
            RegisterAccountResponse response = await accountService.RegisterAccountAsync(requestDto);
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

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<GetMeAccountResponse>> GetMeAccountAsync()
        {
            GetMeAccountResponse response = await accountService.GetMeAccountAsync(User);
            if (!response.Success)
            {
                return Unauthorized(response);
            }
            return Ok(response);
        }
    }
}
