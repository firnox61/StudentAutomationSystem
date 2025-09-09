using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentAutomation.Application.DTOs.Attendances;
using StudentAutomation.Application.Interfaces.Services.Contracts;

namespace StudentAutomation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
    public class AttendancesController : ControllerBase
    {
        private readonly IAttendanceService _svc;
        public AttendancesController(IAttendanceService svc) => _svc = svc;

        [HttpPost("upsert")]
      //  [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Upsert([FromBody] AttendanceUpsertDto dto)
        {
            var r = await _svc.UpsertAsync(dto);
            return r.Success ? Ok(r.Message) : BadRequest(r.Message);
        }

        [HttpDelete("{id}")]
       // [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Delete(int id)
        {
            var r = await _svc.DeleteAsync(id);
            return r.Success ? Ok(r.Message) : BadRequest(r.Message);
        }

        [HttpGet("by-student")]
        public async Task<IActionResult> ByStudent([FromQuery] int studentId, [FromQuery] int? courseId)
        {
            var r = await _svc.GetByStudentAsync(studentId, courseId);
            return r.Success ? Ok(r.Data) : BadRequest(r.Message);
        }

        [HttpGet("by-course/{courseId}")]
        public async Task<IActionResult> ByCourse(int courseId, [FromQuery] DateOnly? date)
        {
            var r = await _svc.GetByCourseAsync(courseId, date);
            return r.Success ? Ok(r.Data) : BadRequest(r.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var r = await _svc.GetByIdAsync(id);
            return r.Success ? Ok(r.Data) : NotFound(r.Message);
        }
    }
}
