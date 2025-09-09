using StudentAutomation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.Repositories
{
    public interface IAttendanceDal : IEntityRepository<Attendance>
    {
        Task<Attendance?> GetByIdDetailAsync(int id);         // Course + Student.User
        Task<Attendance?> GetByKeysAsync(int studentId, int courseId, DateOnly date);
        Task<List<Attendance>> GetByStudentAsync(int studentId, int? courseId);
        Task<List<Attendance>> GetByCourseAsync(int courseId, DateOnly? date);
    }

}
