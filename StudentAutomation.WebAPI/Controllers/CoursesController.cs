using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentAutomation.Application.DTOs.Courses;
using StudentAutomation.Application.Interfaces.Services.Contracts;

namespace StudentAutomation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _svc;
        public CoursesController(ICourseService svc) => _svc = svc;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = true)
        {
            var r = await _svc.GetAllAsync(onlyActive);
            return r.Success ? Ok(r.Data) : BadRequest(r.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var r = await _svc.GetByIdAsync(id);
            return r.Success ? Ok(r.Data) : NotFound(r.Message);
        }

        [HttpPost]
//[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] CourseCreateDto dto)
        {
            var r = await _svc.AddAsync(dto);
            return r.Success ? Ok(r.Message) : BadRequest(r.Message);
        }

        [HttpPut]
     //   [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] CourseUpdateDto dto)
        {
            var r = await _svc.UpdateAsync(dto);
            return r.Success ? Ok(r.Message) : BadRequest(r.Message);
        }

        [HttpDelete("{id}")]
//[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var r = await _svc.DeleteAsync(id);
            return r.Success ? Ok(r.Message) : BadRequest(r.Message);
        }

        [HttpPut("{courseId}/assign-teacher")]
      //  [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignTeacher(int courseId, [FromQuery] int? teacherId)
        {
            var r = await _svc.AssignTeacherAsync(courseId, teacherId);
            return r.Success ? Ok(r.Message) : BadRequest(r.Message);
        }

        [HttpPut("{courseId}/set-active")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetActive(int courseId, [FromQuery] bool isActive)
        {
            var r = await _svc.SetActiveAsync(courseId, isActive);
            return r.Success ? Ok(r.Message) : BadRequest(r.Message);
        }

        [HttpGet("{courseId}/students")]
//[Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetStudents(int courseId)
        {
            var r = await _svc.GetStudentsAsync(courseId);
            return r.Success ? Ok(r.Data) : BadRequest(r.Message);
        }
    }
}
