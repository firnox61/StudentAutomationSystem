using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentAutomation.Application.DTOs.Feedbacks;
using StudentAutomation.Application.Interfaces.Services.Contracts;
using StudentAutomation.Core.Utilities.Results;
using System.Security.Claims;

namespace StudentAutomation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentFeedbacksController : ControllerBase
    {
        private readonly IStudentFeedbackService _service;
        public StudentFeedbacksController(IStudentFeedbackService service) => _service = service;

        [HttpGet("by-course/{courseId:int}")]
        public async Task<IActionResult> GetByCourse(int courseId)
        {
            var result = await _service.GetByCourseAsync(courseId);
            return result.Success ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpGet("by-student/{studentId:int}")]
        public async Task<IActionResult> GetByStudent(int studentId, [FromQuery] int? courseId = null)
        {
            var result = await _service.GetByStudentAsync(studentId, courseId);
            return result.Success ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StudentFeedbackCreateDto dto, [FromQuery] int teacherId)
        {
            var result = await _service.CreateAsync(teacherId, dto);
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] StudentFeedbackUpdateDto dto, [FromQuery] int teacherId)
        {
            var result = await _service.UpdateAsync(teacherId, dto);
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] int teacherId)
        {
            var result = await _service.DeleteAsync(teacherId, id);
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
        }
    }
}
