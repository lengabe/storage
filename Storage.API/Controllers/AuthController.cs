using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Storage.API.Core.Interfaces;
using Storage.API.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Storage.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private static IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        // Post api/auth (login)
        [HttpPost("")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            var result = await _authService.Login(login);
            if (result.Succeeded)
                return Ok(result.Value);
            return BadRequest(result.Errors);
        }
    }
}
