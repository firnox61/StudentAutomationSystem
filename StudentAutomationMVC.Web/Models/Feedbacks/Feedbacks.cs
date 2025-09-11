using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomationMVC.Web.Models.Feedbacks
{
    public class StudentFeedbackCreateDto
    {
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public string Comment { get; set; } = default!;
    }
    public class StudentFeedbackUpdateDto
    {
        public int Id { get; set; }
        public string Comment { get; set; } = default!;
    }
    public class StudentFeedbackListDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public int TeacherId { get; set; }
        public string TeacherFullName { get; set; } = default!;
        public string Comment { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
