using StudentAutomation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.Repositories
{
    public interface IGradeDal : IEntityRepository<Grade>
    {
        Task<Grade?> GetByIdDetailAsync(int id);              // Course + Student.User
        Task<Grade?> GetByKeysAsync(int studentId, int courseId, GradeType type);
        Task<List<Grade>> GetByStudentAsync(int studentId, int? courseId);
        Task<List<Grade>> GetByCourseAsync(int courseId);
        Task<List<Grade>> GetByStudentAsync(int studentId, string? term = null, int? courseId = null);
        Task<List<Grade>> GetByCourseAsync(int courseId, string? term = null, int? studentId = null);
    }

}
