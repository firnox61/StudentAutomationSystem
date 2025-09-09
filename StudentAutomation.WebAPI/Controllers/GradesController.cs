using Autofac.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentAutomation.Application.DTOs.Grades;
using StudentAutomation.Application.Interfaces.Services.Contracts;

namespace StudentAutomation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
    public class GradesController : ControllerBase
    {
        private readonly IGradeService _svc;
        public GradesController(IGradeService svc) => _svc = svc;

        [HttpPost("upsert")]
     //   [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Upsert([FromBody] GradeUpsertDto dto)
        {
            var r = await _svc.UpsertAsync(dto);
            return r.Success ? Ok(r.Message) : BadRequest(r.Message);
        }

        [HttpDelete("{id}")]
      //  [Authorize(Roles = "Admin,Teacher")]
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
        public async Task<IActionResult> ByCourse(int courseId)
        {
            var r = await _svc.GetByCourseAsync(courseId);
            return r.Success ? Ok(r.Data) : BadRequest(r.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var r = await _svc.GetByIdAsync(id);
            return r.Success ? Ok(r.Data) : NotFound(r.Message);
        }
        [HttpGet("grades/average-by-student")]
        public async Task<IActionResult> AverageByStudent([FromQuery] int studentId, [FromQuery] string? term = null)
        {
            var result = await _svc.AverageByStudentAsync(studentId, term);
            return result.Success ? Ok(result.Data) : BadRequest(result.Message);
        }
    }
}
