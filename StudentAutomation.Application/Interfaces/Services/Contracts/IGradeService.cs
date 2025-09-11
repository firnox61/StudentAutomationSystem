using StudentAutomation.Application.DTOs.Courses;
using StudentAutomation.Application.DTOs.Grades;
using StudentAutomation.Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.Interfaces.Services.Contracts
{
    public interface IGradeService
    {
        Task<IResult> UpsertAsync(GradeUpsertDto dto);
        Task<IResult> DeleteAsync(int id);
        Task<IDataResult<List<GradeListDto>>> GetByStudentAsync(int studentId, int? courseId = null);
        Task<IDataResult<List<GradeListDto>>> GetByCourseAsync(int courseId);
        Task<IDataResult<GradeDetailDto>> GetByIdAsync(int id);
        Task<IDataResult<StudentGradeAverageDto>> AverageByStudentAsync(int studentId, string? term = null);
        Task<IDataResult<List<GradeListDto>>> GetAllAsync();
    }
}
