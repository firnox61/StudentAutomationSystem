using StudentAutomation.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace StudentAutomation.Domain.Entities
{
    public class Teacher : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }                 // FK -> User
        public string Title { get; set; } = "Lecturer"; // Dr., Prof. vb.
        public string? Department { get; set; }
        public bool Status { get; set; } = true;

        public User User { get; set; } = default!;
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
