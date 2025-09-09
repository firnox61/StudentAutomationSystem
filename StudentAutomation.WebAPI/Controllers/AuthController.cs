using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentAutomation.Application.DTOs.Users;
using StudentAutomation.Application.Interfaces.Services.Contracts;

namespace StudentAutomation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto, dto.Password);
            if (!result.Success)
                return BadRequest(result.Message);

            var tokenResult = await _authService.CreateAccessTokenAsync(result.Data);
            return Ok(tokenResult.Data); // token objesi dönfbgf
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDto dto)
        {
            var loginResult = await _authService.LoginAsync(dto);
            if (!loginResult.Success)
                return BadRequest(loginResult.Message);

            var tokenResult = await _authService.CreateAccessTokenAsync(loginResult.Data);
            return Ok(tokenResult.Data);
        }

        [HttpGet("check-user")]
        public async Task<IActionResult> CheckUserExists([FromQuery] string email)
        {
            var result = await _authService.UserExistsAsync(email);
            return result.Success ? Ok("Kullanıcı yok, kayıt yapılabilir.") : BadRequest("Kullanıcı zaten var.");
        }
    }
}
