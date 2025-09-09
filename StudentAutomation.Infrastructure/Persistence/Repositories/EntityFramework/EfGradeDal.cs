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
    public class EfGradeDal : EfEntityRepositoryBase<Grade, DataContext>, IGradeDal
    {
        public EfGradeDal(DataContext c) : base(c) { }

        public Task<Grade?> GetByIdDetailAsync(int id) =>
            _context.Grades.AsNoTracking()
                .Include(g => g.Course)
                .Include(g => g.Student).ThenInclude(s => s.User)
                .FirstOrDefaultAsync(g => g.Id == id);

        public Task<Grade?> GetByKeysAsync(int studentId, int courseId, GradeType type) =>
            _context.Grades.FirstOrDefaultAsync(g =>
                g.StudentId == studentId &&
                g.CourseId == courseId &&
                g.Type == type);

        public Task<List<Grade>> GetByStudentAsync(int studentId, int? courseId) =>
            _context.Grades.AsNoTracking()
                .Where(g => g.StudentId == studentId && (!courseId.HasValue || g.CourseId == courseId))
                .Include(g => g.Course)
                .ToListAsync();

        public Task<List<Grade>> GetByCourseAsync(int courseId) =>
            _context.Grades.AsNoTracking()
                .Where(g => g.CourseId == courseId)
                .Include(g => g.Student).ThenInclude(s => s.User)
                .ToListAsync();
        public Task<List<Grade>> GetByStudentAsync(int studentId, string? term = null, int? courseId = null)
        {
            var q = _context.Grades
                .AsNoTracking()
                .Include(g => g.Course)
                .Include(g => g.Student).ThenInclude(s => s.User)
                .Where(g => g.StudentId == studentId);

            if (!string.IsNullOrWhiteSpace(term))
                q = q.Where(g => g.Term == term);

            if (courseId.HasValue)
                q = q.Where(g => g.CourseId == courseId.Value);

            return q.ToListAsync();
        }

        public Task<List<Grade>> GetByCourseAsync(int courseId, string? term = null, int? studentId = null)
        {
            var q = _context.Grades
                .AsNoTracking()
                .Include(g => g.Course)
                .Include(g => g.Student).ThenInclude(s => s.User)
                .Where(g => g.CourseId == courseId);

            if (!string.IsNullOrWhiteSpace(term))
                q = q.Where(g => g.Term == term);

            if (studentId.HasValue)
                q = q.Where(g => g.StudentId == studentId.Value);

            return q.ToListAsync();
        }
    }
}
