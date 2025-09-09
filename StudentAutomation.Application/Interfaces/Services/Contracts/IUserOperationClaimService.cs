using StudentAutomation.Application.DTOs.Users;
using StudentAutomation.Core.Utilities.Results;

namespace StudentAutomation.Application.Interfaces.Services.Contracts
{
    public interface IUserOperationClaimService
    {
        Task<IDataResult<List<UserOperationClaimListDto>>> GetByUserIdAsync(int userId);
        Task<IResult> AddAsync(UserOperationClaimCreateDto dto);
        Task<IResult> DeleteAsync(int id);
    }
}
