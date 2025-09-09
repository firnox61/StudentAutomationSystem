using StudentAutomation.Application.DTOs.Courses;
using StudentAutomation.Application.DTOs.Students;
using StudentAutomation.Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.Interfaces.Services.Contracts
{
    public interface ICourseService
    {
        Task<IDataResult<List<CourseListDto>>> GetAllAsync(bool onlyActive = true);
        Task<IDataResult<CourseDetailDto>> GetByIdAsync(int id);
        Task<IResult> AddAsync(CourseCreateDto dto);
        Task<IResult> UpdateAsync(CourseUpdateDto dto);
        Task<IResult> DeleteAsync(int id);
        Task<IResult> AssignTeacherAsync(int courseId, int? teacherId);
        Task<IResult> SetActiveAsync(int courseId, bool isActive);
        Task<IDataResult<List<StudentMiniDto>>> GetStudentsAsync(int courseId);
    }
}
