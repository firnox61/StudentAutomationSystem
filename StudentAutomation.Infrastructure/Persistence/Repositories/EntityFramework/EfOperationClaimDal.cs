using StudentAutomation.Application.Repositories;
using StudentAutomation.Domain.Entities;
using StudentAutomation.Infrastructure.Persistence.Context;


namespace StudentAutomation.Infrastructure.Persistence.Repositories.EntityFramework
{
    public class EfOperationClaimDal : EfEntityRepositoryBase<OperationClaim, DataContext>, IOperationClaimDal
    {
        public EfOperationClaimDal(DataContext context) : base(context) { }
    }
}
