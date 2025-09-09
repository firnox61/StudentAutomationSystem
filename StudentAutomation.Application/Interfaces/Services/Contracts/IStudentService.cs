using StudentAutomation.Application.DTOs.Students;
using StudentAutomation.Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.Interfaces.Services.Contracts
{
    public interface IStudentService
    {
        Task<IDataResult<List<StudentListDto>>> GetAllAsync();
        Task<IDataResult<StudentDetailDto>> GetByIdAsync(int id);
        Task<IDataResult<StudentDetailDto>> GetByUserIdAsync(int userId);
        Task<IResult> AddAsync(StudentCreateDto dto);
        Task<IResult> UpdateAsync(StudentUpdateDto dto);
        Task<IResult> DeleteAsync(int id);
    }
}