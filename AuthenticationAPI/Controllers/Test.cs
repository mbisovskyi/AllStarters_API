using AuthenticationAPI.DTO;
using AuthenticationAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationAPI.Controllers
{
    public class TestController : BaseApiController
    {
        private readonly IConfiguration config;
        private readonly UserManager<User> userManager;

        public TestController(IConfiguration config, UserManager<User> userManager)
        {
            this.config = config;
            this.userManager = userManager;
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(RegisterAccountDto requestDto)
        {
            User user = new User { UserName = requestDto.UserName, Email = requestDto.Email, CreatedDate = DateTime.Now };
            var result = await userManager.CreateAsync(user, requestDto.Password);
            return Ok();
        }
    }
}
