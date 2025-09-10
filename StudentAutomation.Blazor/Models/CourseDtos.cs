using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Blazor.Models
{
    public class CourseCreateDto
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int Credits { get; set; } = 3;
        public bool IsActive { get; set; } = true;
        public int? TeacherId { get; set; }
    }

    public class CourseUpdateDto : CourseCreateDto
    {
        public int Id { get; set; }
    }

    public class CourseListDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public bool IsActive { get; set; }
        public int? TeacherId { get; set; }
        public string? TeacherFullName { get; set; }
        public int EnrollmentCount { get; set; }
    }

    public class CourseDetailDto : CourseListDto
    {
        public List<StudentMiniDto> Students { get; set; } = new();
    }

    public class CourseMiniDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
    public class CourseBreakdownDto
    {
        public int CourseId { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public decimal? Midterm { get; set; }
        public decimal? Final { get; set; }
        public decimal? Makeup { get; set; }   // varsa final yerine geçer
        public decimal Weighted { get; set; }  // hesaplanan ders notu (0–100)
    }

    public class StudentGradeAverageDto
    {
        public int StudentId { get; set; }
        public string StudentFullName { get; set; } = default!;
        public string? Term { get; set; }

        public decimal Average { get; set; } // tüm derslerin ortalaması (0–100)
        public List<CourseBreakdownDto> Courses { get; set; } = new();
    }
}
