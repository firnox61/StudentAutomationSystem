using StudentAutomation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.Repositories
{
    public interface ICourseDal : IEntityRepository<Course>
    {
        Task<List<Course>> GetAllWithTeacherAsync(bool onlyActive);
        Task<Course?> GetByIdAsync(int id);                   // no include
        Task<Course?> GetByIdFullAsync(int id);               // Teacher + Enrollments + Student.User
        Task<bool> ExistsByCodeAsync(string code, int? excludeId = null);
        Task<bool> IsActiveAsync(int id);
        Task<List<Student>> GetStudentsOfCourseAsync(int courseId);
    }
}
