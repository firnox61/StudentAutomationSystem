using StudentAutomation.Domain.Entities;

namespace StudentAutomation.Application.Repositories
{
    public interface IUserDal : IEntityRepository<User>
    {
        Task<List<OperationClaim>> GetClaimsAsync(User user);
        Task<bool> HasAnyClaimAsync(int userId);
    }
}
