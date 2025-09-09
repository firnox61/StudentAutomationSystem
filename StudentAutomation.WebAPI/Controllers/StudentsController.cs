using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentAutomation.Application.DTOs.Students;
using StudentAutomation.Application.Interfaces.Services.Contracts;

namespace StudentAutomation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _svc;
        public StudentsController(IStudentService svc) => _svc = svc;

        [HttpGet]
       // [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetAll()
        {
            var r = await _svc.GetAllAsync();
            return r.Success ? Ok(r.Data) : BadRequest(r.Message);
        }

        [HttpGet("{id}")]
      //  [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetById(int id)
        {
            var r = await _svc.GetByIdAsync(id);
            return r.Success ? Ok(r.Data) : NotFound(r.Message);
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var r = await _svc.GetByUserIdAsync(userId);
            return r.Success ? Ok(r.Data) : NotFound(r.Message);
        }

        [HttpPost]
      //  [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Add([FromBody] StudentCreateDto dto)
        {
            var r = await _svc.AddAsync(dto);
            return r.Success ? Ok(r.Message) : BadRequest(r.Message);
        }

        [HttpPut]
      //  [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Update([FromBody] StudentUpdateDto dto)
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
