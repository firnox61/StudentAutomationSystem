using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomationMVC.Web.Models.Attendances
{
    public enum AttendanceStatus { Present = 1, Absent = 2, Excused = 3 }
    public class AttendanceUpsertDto
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateOnly Date { get; set; }
        public int? Week { get; set; }
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
        public string? Note { get; set; }
    }

    public class AttendanceListDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateOnly Date { get; set; }
        public int? Week { get; set; }
        public AttendanceStatus Status { get; set; }
        public string? StudentFullName { get; set; }
        public string CourseCode { get; set; } = default!;
        public string CourseName { get; set; } = default!;
        public string? Note { get; set; }
    }

    public class AttendanceDetailDto : AttendanceListDto { }
}
