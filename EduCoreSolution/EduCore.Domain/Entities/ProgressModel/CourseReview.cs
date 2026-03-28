using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.CourseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.ProgressModel
{
    public class CourseReview : BaseEntity<int>
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public byte Rating { get; set; }   // 1–5
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public User Student { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}
