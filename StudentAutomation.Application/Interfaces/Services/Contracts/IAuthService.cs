using StudentAutomation.Application.DTOs.Users;
using StudentAutomation.Core.Utilities.Results;
using StudentAutomation.Domain.Entities;
using StudentAutomation.Domain.Security;


namespace StudentAutomation.Application.Interfaces.Services.Contracts
{
    public interface IAuthService
    {
        Task<IDataResult<User>> RegisterAsync(UserForRegisterDto registerDto, string password);
        Task<IDataResult<User>> LoginAsync(UserForLoginDto loginDto);
        Task<IResult> UserExistsAsync(string email);
        Task<IDataResult<AccessToken>> CreateAccessTokenAsync(User user);
    }
}
