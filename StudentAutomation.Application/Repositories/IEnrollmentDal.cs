using StudentAutomation.Application.DTOs.Students;
using StudentAutomation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.Repositories
{
    public interface IEnrollmentDal : IEntityRepository<Enrollment>
    {
        Task<Enrollment?> GetAsync(int studentId, int courseId);
        Task<List<Course>> GetCoursesOfStudentAsync(int studentId);

        // yeni, projection döner
        Task<List<StudentMiniDto>> GetStudentMinisOfCourseAsync(int courseId);

    }
}
