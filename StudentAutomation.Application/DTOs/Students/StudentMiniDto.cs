using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.DTOs.Students
{
    public class StudentMiniDto
    {
        public int Id { get; set; }
        public string StudentNumber { get; set; } = default!;
        public string FullName { get; set; } = default!;
    }
}
