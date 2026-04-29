using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Reviews
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; } 
        public int CourseId { get; set; }
        public string CourseName { get; set; }   
        public byte Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
