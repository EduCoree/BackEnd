using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.CourseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.ProgressModel
{
    public class LessonProgress : BaseEntity<int>
    {
        public int StudentId { get; set; }
        public int LessonId { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        public int? LastPositionSecs { get; set; }  // video resume point

        // Navigation
        public User Student { get; set; } = null!;
        public Lesson Lesson { get; set; } = null!;
    }
}
