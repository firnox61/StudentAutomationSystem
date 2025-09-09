using StudentAutomation.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace StudentAutomation.Domain.Entities
{
    public enum AttendanceStatus { Present = 1, Absent = 2, Excused = 3 }

    public class Attendance : IEntity
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateOnly Date { get; set; }              // Npgsql EF Core'da 'date' olarak desteklenir
        public int? Week { get; set; }
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
        public string? Note { get; set; }

        public Student Student { get; set; } = default!;
        public Course Course { get; set; } = default!;
    }
}
