using StudentAutomation.Application.DTOs.Teachers;
using StudentAutomation.Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.Interfaces.Services.Contracts
{
    public interface ITeacherService
    {
        Task<IDataResult<List<TeacherListDto>>> GetAllAsync();
        Task<IDataResult<TeacherDetailDto>> GetByIdAsync(int id);
        Task<IDataResult<TeacherDetailDto>> GetByUserIdAsync(int userId);
        Task<IResult> AddAsync(TeacherCreateDto dto);
        Task<IResult> UpdateAsync(TeacherUpdateDto dto);
        Task<IResult> DeleteAsync(int id);
    }
}
