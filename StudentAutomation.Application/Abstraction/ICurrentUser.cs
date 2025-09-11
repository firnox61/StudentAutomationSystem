using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.Abstraction
{
    public interface ICurrentUser
    {
        int? UserId { get; }
        string? Email { get; }
        IReadOnlyCollection<string> Roles { get; }
        bool IsInRole(string role);
    }
}
