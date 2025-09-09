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
    public class EfStudentDal : EfEntityRepositoryBase<Student, DataContext>, IStudentDal
    {
        public EfStudentDal(DataContext c) : base(c) { }

        public Task<List<Student>> GetAllWithUserAsync() =>
            _context.Students.AsNoTracking()
                .Include(s => s.User)
                .ToListAsync();

        public Task<Student?> GetByIdAsync(int id) =>
            _context.Students.FirstOrDefaultAsync(s => s.Id == id);

        public Task<Student?> GetByIdWithUserAsync(int id) =>
            _context.Students.AsNoTracking()
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

        public Task<Student?> GetByUserIdWithUserAsync(int userId) =>
            _context.Students.AsNoTracking()
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);

        public async Task<bool> ExistsByStudentNumberAsync(string studentNumber, int? excludeId = null)
        {
            var q = _context.Students.AsNoTracking().Where(s => s.StudentNumber == studentNumber);
            if (excludeId.HasValue) q = q.Where(s => s.Id != excludeId.Value);
            return await q.AnyAsync();
        }
    }
}
