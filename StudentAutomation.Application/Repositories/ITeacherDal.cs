using StudentAutomation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.Repositories
{
    public interface ITeacherDal : IEntityRepository<Teacher>
    {
        Task<List<Teacher>> GetAllWithUserAsync();
        Task<Teacher?> GetByIdAsync(int id);                  // no include
        Task<Teacher?> GetByIdWithUserAsync(int id);
        Task<Teacher?> GetByUserIdWithUserAsync(int userId);
    }
}
