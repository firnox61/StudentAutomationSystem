using StudentAutomation.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Domain.Entities
{
    public class StudentFeedback : IEntity
    {
        public int Id { get; set; }

        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public int TeacherId { get; set; }     // Yorumu yazan öğretmen

        public string Comment { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Course Course { get; set; } = default!;
        public Student Student { get; set; } = default!;
        public Teacher Teacher { get; set; } = default!;
    }
}
