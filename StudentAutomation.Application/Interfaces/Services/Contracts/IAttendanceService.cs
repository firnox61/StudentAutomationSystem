using StudentAutomation.Application.DTOs.Attendances;
using StudentAutomation.Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.Interfaces.Services.Contracts
{
    public interface IAttendanceService
    {
        Task<IResult> UpsertAsync(AttendanceUpsertDto dto);
        Task<IResult> DeleteAsync(int id);
        Task<IDataResult<List<AttendanceListDto>>> GetByStudentAsync(int studentId, int? courseId = null);
        Task<IDataResult<List<AttendanceListDto>>> GetByCourseAsync(int courseId, DateOnly? date = null);
        Task<IDataResult<AttendanceDetailDto>> GetByIdAsync(int id);
    }
}

