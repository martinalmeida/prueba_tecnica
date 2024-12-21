using Microsoft.AspNetCore.Mvc;
using prueba_tecnica.Services;

namespace prueba_tecnica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LdapController : ControllerBase
    {
        private readonly LdapService _ldapService;

        public LdapController(LdapService ldapService)
        {
            _ldapService = ldapService;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Username and Password are required");
            }

            var ldapEntry = _ldapService.Authenticate(request.Username, request.Password);
            if (ldapEntry == null)
            {
                return Unauthorized("Invalid credentials");
            }

            // Generar el token JWT
            var token = _ldapService.GenerateToken(ldapEntry);

            return Ok(new { Token = token });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
