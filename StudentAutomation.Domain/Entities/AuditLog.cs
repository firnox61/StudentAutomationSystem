using StudentAutomation.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Domain.Entities
{
    public class AuditLog : IEntity
    {
        public int Id { get; set; }
        public string UserEmail { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string Controller { get; set; } = null!;
        public string Method { get; set; } = null!;
        public DateTime Timestamp { get; set; }
    }
}
