using StudentAutomation.Core.Services;
using StudentAutomation.Domain.Entities;
using StudentAutomation.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StudentAutomation.Infrastructure.Aspects.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly DataContext _context;

        public AuditLogService(DataContext context)
        {
            _context = context;
        }

        public void Log(string userEmail, string controller, string method)
        {
            var log = new AuditLog
            {
                UserEmail = userEmail,
                Controller = controller,
                Method = method,
                Action = $"{method} executed",
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            _context.SaveChanges();
        }
    }

}
