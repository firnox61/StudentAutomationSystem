using Microsoft.EntityFrameworkCore;
using StudentAutomation.Application.Repositories;
using StudentAutomation.Domain.Entities;
using StudentAutomation.Infrastructure.Persistence.Context;


namespace StudentAutomation.Infrastructure.Persistence.Repositories.EntityFramework
{
    public class EfUserDal : EfEntityRepositoryBase<User, DataContext>, IUserDal
    {

        public EfUserDal(DataContext context) : base(context) { }

        public async Task<List<OperationClaim>> GetClaimsAsync(User user)
        {
            var claims = await _context.UserOperationClaims
                .Where(uoc => uoc.UserId == user.Id)
                .Include(uoc => uoc.OperationClaim)
                .Select(uoc => uoc.OperationClaim)
                .ToListAsync();

            return claims;
        }
        public async Task<bool> HasAnyClaimAsync(int userId)
        {
            // UserOperationClaims tablosunu doğrudan sorgulamak en hızlısı
            return await _context.UserOperationClaims
                                 .AsNoTracking()
                                 .AnyAsync(uoc => uoc.UserId == userId);
        }
    }
}
