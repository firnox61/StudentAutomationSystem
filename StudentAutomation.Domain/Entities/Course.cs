using StudentAutomation.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Domain.Entities
{
    public class Course : IEntity
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;    // unique
        public string Name { get; set; } = default!;
        public int? TeacherId { get; set; }             // course owner (opsiyonel)
        public int Credits { get; set; } = 3;
        public bool IsActive { get; set; } = true;

        public Teacher? Teacher { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
