using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Course>> GetCoursesOfStudentAsync(int studentId) =>
            await _context.Enrollments.AsNoTracking()
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course)
                .Select(e => e.Course)
                .ToListAsync();

        public async Task<List<Student>> GetStudentsOfCourseAsync(int courseId) =>
            await _context.Enrollments.AsNoTracking()
                .Where(e => e.CourseId == courseId)
                .Include(e => e.Student).ThenInclude(s => s.User)
                .Select(e => e.Student)
                .ToListAsync();
    }
}
