using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomationMVC.Web.Models.Teachers
{
    public class TeacherCreateDto
    {
        public int UserId { get; set; }
        public string Title { get; set; } = "Lecturer";
        public string? Department { get; set; }
        public bool Status { get; set; } = true;
    }

    public class TeacherUpdateDto : TeacherCreateDto
    {
        public int Id { get; set; }
    }

    public class TeacherListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Department { get; set; }
        public bool Status { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
    }

    public class TeacherDetailDto : TeacherListDto { }
}
