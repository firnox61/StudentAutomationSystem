using StudentAutomation.Core.Security;

namespace StudentAutomation.Infrastructure.Security.Hashing
{
    public class HashingService : IHashingService
    {
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            return HashingHelper.VerifyPasswordHash(password, passwordHash, passwordSalt);
        }
    }
}
