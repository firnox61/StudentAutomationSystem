using StudentAutomation.Domain.Entities;

namespace StudentAutomation.Domain.Security
{
    public interface ITokenHelper
    {
        AccessToken CreateToken(User user, List<OperationClaim> operationClaims);//ilgili kullanıcının ilgili claimlerini üretecek
    }
}
