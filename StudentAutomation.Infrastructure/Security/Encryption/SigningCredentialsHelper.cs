using Microsoft.IdentityModel.Tokens;

namespace StudentAutomation.Infrastructure.Security.Encryption
{
    public class SigningCredentialsHelper
    {//json weptokenlarının oluşturulabilmesi sistemi kullanabilmek için elimizde olanlar burası .netcore için
        public static SigningCredentials CreateSigningCredentials(SecurityKey securityKey)
        {
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);//hangi anahtar ve hangi algoritmayo kullanacağını veriyoruz
        }
    }
}
