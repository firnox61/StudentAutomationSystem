using StudentAutomation.Domain.Entities;

namespace StudentAutomation.Application.Repositories
{
    public interface IUserOperationClaimDal : IEntityRepository<UserOperationClaim>
    {
        Task<List<UserOperationClaim>> GetWithDetailsByUserIdAsync(int userId);
    }
}
