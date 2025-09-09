using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.DTOs.Students
{
    public class StudentCreateDto
    {
        public int UserId { get; set; }
        public string StudentNumber { get; set; } = default!;
        public string? Department { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool Status { get; set; } = true;
    }
}
