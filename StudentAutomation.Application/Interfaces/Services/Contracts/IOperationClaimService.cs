
using StudentAutomation.Application.DTOs.Users;
using StudentAutomation.Core.Utilities.Results;

namespace StudentAutomation.Application.Interfaces.Services.Contracts
{
    public interface IOperationClaimService
    {
        Task<IDataResult<List<OperationClaimListDto>>> GetAllAsync();
        Task<IDataResult<OperationClaimListDto>> GetByIdAsync(int id);
        Task<IResult> AddAsync(OperationClaimCreateDto dto);
        Task<IResult> UpdateAsync(OperationClaimUpdateDto dto); // ✅ güncel hali
        Task<IResult> DeleteAsync(int id);
    }
}
