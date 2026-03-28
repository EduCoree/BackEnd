using EduCore.Domain.Entities.CourseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.ContentModel
{
    public class PdfLesson:BaseEntity<int>
    {
        public int LessonId { get; set; }
        public string FileUrl { get; set; } = null!;
        public int? FileSizeKb { get; set; }

        // Navigation
        public Lesson Lesson { get; set; } = null!;
    }
}
