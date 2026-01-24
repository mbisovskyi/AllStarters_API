
using AuthenticationAPI.Models;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Policy = "Admin")]
        [HttpGet]
        public IActionResult Get()
        {
            var token = Request.Headers["Authorization"].ToString();
            return Ok(new {RequestToken = token});
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] object test)
        {
            return Ok();
        }
    }
}
