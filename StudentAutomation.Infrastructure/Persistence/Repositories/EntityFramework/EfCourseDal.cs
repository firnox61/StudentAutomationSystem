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
    public class EfCourseDal : EfEntityRepositoryBase<Course, DataContext>, ICourseDal
    {
        public EfCourseDal(DataContext c) : base(c) { }

        public Task<List<Course>> GetAllWithTeacherAsync(bool onlyActive) =>
            _context.Courses.AsNoTracking()
                .Where(c => !onlyActive || c.IsActive)
                .Include(c => c.Teacher)!.ThenInclude(t => t!.User)
                .ToListAsync();

        public Task<Course?> GetByIdAsync(int id) =>
            _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

        public Task<Course?> GetByIdFullAsync(int id) =>
            _context.Courses.AsNoTracking()
                .Include(c => c.Teacher)!.ThenInclude(t => t!.User)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student).ThenInclude(s => s.User)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<bool> ExistsByCodeAsync(string code, int? excludeId = null)
        {
            var q = _context.Courses.AsNoTracking().Where(c => c.Code == code);
            if (excludeId.HasValue) q = q.Where(c => c.Id != excludeId.Value);
            return await q.AnyAsync();
        }

        public async Task<bool> IsActiveAsync(int id) =>
            await _context.Courses.AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => c.IsActive)
                .FirstOrDefaultAsync();

        public async Task<List<Student>> GetStudentsOfCourseAsync(int courseId) =>
            await _context.Enrollments.AsNoTracking()
                .Where(e => e.CourseId == courseId)
                .Include(e => e.Student).ThenInclude(s => s.User)
                .Select(e => e.Student)
                .ToListAsync();
    }
}
