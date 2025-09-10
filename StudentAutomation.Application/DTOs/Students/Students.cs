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
    public class StudentUpdateDto : StudentCreateDto
    {
        public int Id { get; set; }
    }
    public class StudentMiniDto
    {
        public int Id { get; set; }
        public string StudentNumber { get; set; } = default!;
        public string FullName { get; set; } = default!;
    }
    public class StudentDetailDto : StudentListDto
    {
        public DateTime? BirthDate { get; set; }
    }
    public class StudentListDto
    {
        public int Id { get; set; }
        public string StudentNumber { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Department { get; set; }
        public bool Status { get; set; }
    }
}
