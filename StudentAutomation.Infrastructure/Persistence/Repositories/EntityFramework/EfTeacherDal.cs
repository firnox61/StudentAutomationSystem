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
    public class EfTeacherDal : EfEntityRepositoryBase<Teacher, DataContext>, ITeacherDal
    {
        public EfTeacherDal(DataContext c) : base(c) { }

        public Task<List<Teacher>> GetAllWithUserAsync() =>
            _context.Teachers.AsNoTracking()
                .Include(t => t.User)
                .ToListAsync();

        public Task<Teacher?> GetByIdAsync(int id) =>
            _context.Teachers.FirstOrDefaultAsync(t => t.Id == id);

        public Task<Teacher?> GetByIdWithUserAsync(int id) =>
            _context.Teachers.AsNoTracking()
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);

        public Task<Teacher?> GetByUserIdWithUserAsync(int userId) =>
            _context.Teachers.AsNoTracking()
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.UserId == userId);
    }
}
