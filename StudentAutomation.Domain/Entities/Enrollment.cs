using StudentAutomation.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Domain.Entities
{
    // Join (Many-to-Many): Student <-> Course
    public class Enrollment : IEntity
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateOnly? EnrolledAt { get; set; }  // nullable ise ? bırak
        public Student Student { get; set; } = default!;
        public Course Course { get; set; } = default!;
    }
}
