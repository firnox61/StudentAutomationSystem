using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentAutomation.Application.DTOs.Teachers;
using StudentAutomation.Application.Interfaces.Services.Contracts;

namespace StudentAutomation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _svc;
        public TeachersController(ITeacherService svc) => _svc = svc;

        [HttpGet]
      //  [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var r = await _svc.GetAllAsync();
            return r.Success ? Ok(r.Data) : BadRequest(r.Message);
        }

        [HttpGet("{id}")]
      //  [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var r = await _svc.GetByIdAsync(id);
            return r.Success ? Ok(r.Data) : NotFound(r.Message);
        }

        [HttpGet("by-user/{userId}")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var r = await _svc.GetByUserIdAsync(userId);
            return r.Success ? Ok(r.Data) : NotFound(r.Message);
        }

        [HttpPost]
     //   [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] TeacherCreateDto dto)
        {
            var r = await _svc.AddAsync(dto);
            return r.Success ? Ok(r.Message) : BadRequest(r.Message);
        }

        [HttpPut]
     //   [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] TeacherUpdateDto dto)
        {
            var r = await _svc.UpdateAsync(dto);
            return r.Success ? Ok(r.Message) : BadRequest(r.Message);
        }

        [HttpDelete("{id}")]
     //   [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var r = await _svc.DeleteAsync(id);
            return r.Success ? Ok(r.Message) : BadRequest(r.Message);
        }
    }
}
