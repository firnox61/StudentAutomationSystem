using StudentAutomation.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace StudentAutomation.Domain.Entities
{
    public class Student : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }                 // FK -> User
        public string StudentNumber { get; set; } = default!;
        public string? Department { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool Status { get; set; } = true;

        public User User { get; set; } = default!;
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
