using EduCore.Domain.Entities.ContentModel;
using EduCore.Domain.Entities.ForumModel;
using EduCore.Domain.Entities.ProgressModel;
using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.CourseModel
{
    
    public class Lesson: BaseEntity<int>
    {
        public int SectionId { get; set; }
        public string Title { get; set; } = null!;
        public LessonType Type { get; set; }   // flags/SET
        public int SortOrder { get; set; }
        public int? DurationSeconds { get; set; }
        public bool IsFreePreview { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation — Parent
        public Section Section { get; set; } = null!;

        // Navigation — Content (1-to-1 each)
        public VideoLesson? VideoLesson { get; set; }
        public PdfLesson? PdfLesson { get; set; }
        public LiveSession? LiveSession { get; set; }

        // Navigation — Progress
        public ICollection<LessonProgress> Progresses { get; set; } = new List<LessonProgress>();

        // Navigation — Forum
        public ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();
    }
}
