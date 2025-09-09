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
    public class EfStudentFeedbackDal : EfEntityRepositoryBase<StudentFeedback, DataContext>, IStudentFeedbackDal
    {
        public EfStudentFeedbackDal(DataContext c) : base(c) { }

        public Task<StudentFeedback?> GetByIdFullAsync(int id) =>
            _context.StudentFeedbacks
                .AsNoTracking()
                .Include(f => f.Teacher).ThenInclude(t => t.User)
                .Include(f => f.Student).ThenInclude(s => s.User)
                .Include(f => f.Course)
                .AsSplitQuery()
                .FirstOrDefaultAsync(f => f.Id == id);

        public Task<List<StudentFeedback>> GetByCourseAsync(int courseId) =>
            _context.StudentFeedbacks
                .AsNoTracking()
                .Include(f => f.Teacher).ThenInclude(t => t.User)
                .Include(f => f.Student).ThenInclude(s => s.User)
                .Include(f => f.Course)
                .AsSplitQuery()
                .Where(f => f.CourseId == courseId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

        public Task<List<StudentFeedback>> GetByStudentAsync(int studentId, int? courseId = null)
        {
            var q = _context.StudentFeedbacks
                .AsNoTracking()
                .Include(f => f.Teacher).ThenInclude(t => t.User)
                .Include(f => f.Student).ThenInclude(s => s.User)
                .Include(f => f.Course)
                .AsSplitQuery()
                .Where(f => f.StudentId == studentId);

            if (courseId.HasValue)
                q = q.Where(f => f.CourseId == courseId.Value);

            return q.OrderByDescending(f => f.CreatedAt).ToListAsync();
        }

        public Task<List<StudentFeedback>> GetMineAsync(int teacherId, int? studentId = null, int? courseId = null)
        {
            var q = _context.StudentFeedbacks
                .AsNoTracking()
                .Include(f => f.Teacher).ThenInclude(t => t.User)
                .Include(f => f.Student).ThenInclude(s => s.User)
                .Include(f => f.Course)
                .AsSplitQuery()
                .Where(f => f.TeacherId == teacherId);

            if (studentId.HasValue)
                q = q.Where(f => f.StudentId == studentId.Value);

            if (courseId.HasValue)
                q = q.Where(f => f.CourseId == courseId.Value);

            return q.OrderByDescending(f => f.CreatedAt).ToListAsync();
        }
    }
}
