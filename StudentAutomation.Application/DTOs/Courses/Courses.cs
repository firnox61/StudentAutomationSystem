using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.DTOs.Courses
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
        public int Credits { get; set; }
        public bool IsActive { get; set; }
        public int? TeacherId { get; set; }
        public string? TeacherName { get; set; }
    }

    public class CourseDetailDto : CourseListDto
    {
        public List<StudentAutomation.Application.DTOs.Students.StudentMiniDto> Students { get; set; } = new();
    }

    public class CourseMiniDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}
