using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.CourseDTOs
{
    public class StudentEnrolledCourseDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? CoverImage { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public DateTime EnrolledAt { get; set; }
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }

        //  DB Computed
        public double ProgressPercentage =>
            TotalLessons == 0 ? 0 : Math.Round((double)CompletedLessons / TotalLessons * 100, 1);
    }
}
