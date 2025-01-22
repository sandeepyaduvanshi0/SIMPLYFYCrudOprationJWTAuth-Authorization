using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Models.DTOs;
using Server.Repository.IRepository;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUserAccount accountInterface) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> CreateAsync(Register user)
        {
            if (user == null) return BadRequest("Model is empty");
            var result = await accountInterface.RegisterAsync(user);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> SignInAsync(Login user)
        {
            if (user == null) return BadRequest("Model is empty");
            var result = await accountInterface.LoginAsync(user);
            return Ok(result);
        }
    }
}
