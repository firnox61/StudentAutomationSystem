using StudentAutomation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.Repositories
{
    public interface IStudentDal : IEntityRepository<Student>
    {

        Task<List<Student>> GetAllWithUserAsync();
        Task<Student?> GetByIdAsync(int id);                  // no include
        Task<Student?> GetByIdWithUserAsync(int id);
        Task<Student?> GetByUserIdWithUserAsync(int userId);
        Task<bool> ExistsByStudentNumberAsync(string studentNumber, int? excludeId = null);
    }
}
