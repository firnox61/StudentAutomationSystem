using Microsoft.EntityFrameworkCore;
using StudentAutomation.Application.DTOs.Students;
using StudentAutomation.Application.Repositories;
using StudentAutomation.Domain.Entities;
using StudentAutomation.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Infrastructure.Persistence.Repositories.EntityFramework
{
    public class EfEnrollmentDal : EfEntityRepositoryBase<Enrollment, DataContext>, IEnrollmentDal
    {
        public EfEnrollmentDal(DataContext c) : base(c) { }

        public Task<Enrollment?> GetAsync(int studentId, int courseId) =>
            _context.Enrollments.FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

        public Task<List<Course>> GetCoursesOfStudentAsync(int studentId) =>
            _context.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == studentId)
                .Select(e => e.Course)
                .Distinct()
                .OrderBy(c => c.Name)
                .ToListAsync();

        public Task<List<StudentMiniDto>> GetStudentMinisOfCourseAsync(int courseId) =>
            _context.Enrollments
                .AsNoTracking()
                .Where(e => e.CourseId == courseId)
                .Select(e => new StudentMiniDto
                {
                    Id = e.Student.Id,
                    StudentNumber = e.Student.StudentNumber,
                    FullName = e.Student.User.FirstName + " " + e.Student.User.LastName
                })
                .OrderBy(s => s.FullName)
                .ToListAsync();
    }
}
