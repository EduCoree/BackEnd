using EduCore.Domain.Entities.CourseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.ContentModel
{
    public class VideoLesson:BaseEntity<int>
    {
        public int LessonId { get; set; }
        public string VideoUrl { get; set; } = null!;
        public string? VideoProvider { get; set; }  // "youtube","vimeo","bunny"
        public string? ThumbnailUrl { get; set; }
        public string? Transcript { get; set; }     // AI-generated via Whisper
        public DateTime? TranscribedAt { get; set; }

        // Navigation
        public Lesson Lesson { get; set; } = null!;
    }
}
