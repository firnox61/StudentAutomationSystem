using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomationMVC.Web.Models.Grades
{
    public enum GradeType { Midterm = 1, Final = 2, Makeup = 3, Quiz = 4, Project = 5 }
    public class GradeUpsertDto
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public GradeType Type { get; set; }
        public decimal Value { get; set; } // 0..100
        public string? Term { get; set; }
    }

    public class GradeListDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public GradeType Type { get; set; }
        public decimal Value { get; set; }
        public string? Term { get; set; }
        public string? StudentFullName { get; set; }
        public string CourseCode { get; set; } = default!;
        public string CourseName { get; set; } = default!;
    }

    public class GradeDetailDto : GradeListDto { }
}
