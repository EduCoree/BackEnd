using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz.Student
{
    public class HistoryFilterDto
    {
        public string? CourseTitle { get; set; }
        public string? Status { get; set; } 
        public string? DateRange { get; set; } 
    }
}
