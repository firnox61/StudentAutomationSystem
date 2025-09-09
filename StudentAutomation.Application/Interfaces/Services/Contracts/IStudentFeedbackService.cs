using StudentAutomation.Application.DTOs.Feedbacks;
using StudentAutomation.Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.Interfaces.Services.Contracts
{
    public interface IStudentFeedbackService
    {
        Task<IDataResult<List<StudentFeedbackListDto>>> GetByCourseAsync(int courseId);
        Task<IDataResult<List<StudentFeedbackListDto>>> GetByStudentAsync(int studentId, int? courseId = null);

        Task<IResult> CreateAsync(int userId, StudentFeedbackCreateDto dto);
        Task<IResult> UpdateAsync(int userId, StudentFeedbackUpdateDto dto);
        Task<IResult> DeleteAsync(int userId, int id);
    }
}
