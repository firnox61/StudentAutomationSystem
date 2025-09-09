using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Core.Services
{
    public interface IAuditLogService
    {
        void Log(string userEmail, string controller, string method);
    }

}
