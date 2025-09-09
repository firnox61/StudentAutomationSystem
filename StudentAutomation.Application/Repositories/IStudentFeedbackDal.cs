using StudentAutomation.Application.DTOs.Feedbacks;
using StudentAutomation.Core.Utilities.Results;
using StudentAutomation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.Repositories
{
    public interface IStudentFeedbackDal : IEntityRepository<StudentFeedback>
    {
        Task<StudentFeedback?> GetByIdFullAsync(int id);
        Task<List<StudentFeedback>> GetByCourseAsync(int courseId);
        Task<List<StudentFeedback>> GetByStudentAsync(int studentId, int? courseId = null);
        Task<List<StudentFeedback>> GetMineAsync(int teacherId, int? studentId = null, int? courseId = null);
        // CRUD için AddAsync/UpdateAsync/DeleteAsync zaten IEntityRepository'den geliyor.
    }
}
