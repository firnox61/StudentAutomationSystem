using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.DTOs.Users
{
    public class ForgotPasswordRequestDto
    {
        public string Email { get; set; }
 
    }
    public class ResetPasswordRequestDto
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
