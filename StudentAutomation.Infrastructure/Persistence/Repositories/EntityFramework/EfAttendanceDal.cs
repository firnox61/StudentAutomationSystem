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
    public class EfAttendanceDal : EfEntityRepositoryBase<Attendance, DataContext>, IAttendanceDal
    {
        public EfAttendanceDal(DataContext c) : base(c) { }

        public Task<Attendance?> GetByIdDetailAsync(int id) =>
            _context.Attendances.AsNoTracking()
                .Include(a => a.Course)
                .Include(a => a.Student).ThenInclude(s => s.User)
                .FirstOrDefaultAsync(a => a.Id == id);

        public Task<Attendance?> GetByKeysAsync(int studentId, int courseId, DateOnly date) =>
            _context.Attendances.FirstOrDefaultAsync(a =>
                a.StudentId == studentId &&
                a.CourseId == courseId &&
                a.Date == date);

        public Task<List<Attendance>> GetByStudentAsync(int studentId, int? courseId) =>
            _context.Attendances.AsNoTracking()
                .Where(a => a.StudentId == studentId && (!courseId.HasValue || a.CourseId == courseId))
                .Include(a => a.Course)
                .ToListAsync();

        public Task<List<Attendance>> GetByCourseAsync(int courseId, DateOnly? date) =>
            _context.Attendances.AsNoTracking()
                .Where(a => a.CourseId == courseId && (!date.HasValue || a.Date == date.Value))
                .Include(a => a.Student).ThenInclude(s => s.User)
                .ToListAsync();
    }
}