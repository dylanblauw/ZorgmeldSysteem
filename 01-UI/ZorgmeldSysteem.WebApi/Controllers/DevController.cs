using Microsoft.AspNetCore.Mvc;

namespace ZorgmeldSysteem.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevController : ControllerBase
    {
        [HttpGet("generate-hash/{password}")]
        public IActionResult GenerateHash(string password)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(password, 11);
            var verify = BCrypt.Net.BCrypt.Verify(password, hash);

            return Ok(new
            {
                password = password,
                hash = hash,
                verified = verify,
                message = "Kopieer de hash en gebruik in SQL UPDATE"
            });
        }

        [HttpPost("test-hash")]
        public IActionResult TestHash([FromBody] HashTestRequest request)
        {
            var isValid = BCrypt.Net.BCrypt.Verify(request.Password, request.Hash);

            return Ok(new
            {
                password = request.Password,
                hash = request.Hash,
                isValid = isValid
            });
        }
    }

    public class HashTestRequest
    {
        public string Password { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;
    }
}