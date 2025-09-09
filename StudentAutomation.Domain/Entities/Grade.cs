using StudentAutomation.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace StudentAutomation.Domain.Entities
{
    public enum GradeType { Midterm = 1, Final = 2, Makeup = 3, Quiz = 4, Project = 5 }

    public class Grade : IEntity
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public GradeType Type { get; set; } = GradeType.Final;
        public decimal Value { get; set; }              // 0–100 arası
        public string? Term { get; set; }               // 2025-FALL gibi
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Student Student { get; set; } = default!;
        public Course Course { get; set; } = default!;
    }
}
