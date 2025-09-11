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
    public interface IEnrollmentService
    {
        Task<IResult> EnrollAsync(int studentId, int courseId, DateOnly? enrolledAt = null);
        Task<IResult> UnenrollAsync(int studentId, int courseId);
        Task<IDataResult<List<CourseMiniDto>>> GetCoursesOfStudentAsync(int studentId);
        Task<IDataResult<List<StudentMiniDto>>> GetStudentsOfCourseAsync(int courseId);
    }
}
