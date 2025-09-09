using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentAutomation.Application.DTOs.Users;
using StudentAutomation.Application.Interfaces.Services.Contracts;

namespace StudentAutomation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return result.Success ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userService.GetByIdAsync(id);
            return result.Success ? Ok(result.Data) : NotFound(result.Message);
        }

        [HttpGet("by-email")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            var result = await _userService.GetByEmailAsync(email);
            return result.Success ? Ok(result.Data) : NotFound(result.Message);
        }
        [HttpPut("edit-profile")]
        public async Task<IActionResult> EditProfile([FromBody] UserDto userDto, string password)
        {
            var result = await _userService.EditProfil(userDto, password);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] UserCreateDto dto)
        {
            var result = await _userService.AddAsync(dto);
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto dto)
        {
            var result = await _userService.UpdateAsync(dto);
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.DeleteAsync(id);
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
        }
    }
}
