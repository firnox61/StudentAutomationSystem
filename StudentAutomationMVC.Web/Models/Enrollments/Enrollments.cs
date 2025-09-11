using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomationMVC.Web.Models.Enrollments
{
    public class EnrollRequestDto
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateOnly? EnrolledAt { get; set; }
    }
}
