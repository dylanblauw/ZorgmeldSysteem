using Microsoft.AspNetCore.Mvc;
using ZorgmeldSysteem.Application.DTOs.Auth;
using ZorgmeldSysteem.Persistence.Services;
using ZorgmeldSysteem.WebApi.Controllers;

namespace ZorgmeldSysteem.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(loginDto);

            if (result == null)
                return Unauthorized(new { message = "Onjuiste email of wachtwoord" });

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(registerDto);

            if (result == null)
                return BadRequest(new { message = "Email bestaat al of bedrijf niet gevonden" });

            return Ok(result);
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new
            {
                message = "Auth API werkt!",
                timestamp = DateTime.Now
            });
        }
    }
}
