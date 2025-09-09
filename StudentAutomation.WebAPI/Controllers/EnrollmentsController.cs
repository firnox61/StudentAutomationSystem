using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentAutomation.Application.DTOs.Enrollments;
using StudentAutomation.Application.Interfaces.Services.Contracts;

namespace StudentAutomation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(Roles = "Admin,Teacher")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _svc;
        public EnrollmentsController(IEnrollmentService svc) => _svc = svc;

        [HttpPost("enroll")]
        public async Task<IActionResult> Enroll([FromBody] EnrollRequestDto dto)
        {
            var r = await _svc.EnrollAsync(dto.StudentId, dto.CourseId, dto.EnrolledAt);
            return r.Success ? Ok(r.Message) : BadRequest(r.Message);
        }

        [HttpDelete("unenroll")]
        public async Task<IActionResult> Unenroll([FromQuery] int studentId, [FromQuery] int courseId)
        {
            var r = await _svc.UnenrollAsync(studentId, courseId);
            return r.Success ? Ok(r.Message) : BadRequest(r.Message);
        }

        [HttpGet("student/{studentId}/courses")]
        public async Task<IActionResult> GetCoursesOfStudent(int studentId)
        {
            var r = await _svc.GetCoursesOfStudentAsync(studentId);
            return r.Success ? Ok(r.Data) : BadRequest(r.Message);
        }

        [HttpGet("course/{courseId}/students")]
        public async Task<IActionResult> GetStudentsOfCourse(int courseId)
        {
            var r = await _svc.GetStudentsOfCourseAsync(courseId);
            return r.Success ? Ok(r.Data) : BadRequest(r.Message);
        }
    }
}
