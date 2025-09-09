using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.DTOs.Students
{
    public class StudentDetailDto : StudentListDto
    {
        public DateTime? BirthDate { get; set; }
    }
}
