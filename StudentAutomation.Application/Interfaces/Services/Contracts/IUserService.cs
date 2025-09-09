
using StudentAutomation.Application.DTOs.Users;
using StudentAutomation.Core.Utilities.Results;
using StudentAutomation.Domain.Entities;

namespace StudentAutomation.Application.Interfaces.Services.Contracts
{
    public interface IUserService
    {
        Task<IDataResult<List<UserListDto>>> GetAllAsync();
        Task<IResult> EditProfil(UserDto userDto, string password);
        Task<IDataResult<UserListDto>> GetByIdAsync(int id);
        Task<IDataResult<User>> GetByEmailAsync(string email);
        Task<IDataResult<List<OperationClaim>>> GetClaimsAsync(User user);
        Task<IResult> AddAsync(UserCreateDto dto);
        Task<IResult> UpdateAsync(UserUpdateDto dto);
        Task<IResult> DeleteAsync(int id);


    }
}
