using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.CourseDTOs
{
    public class SectionDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public List<LessonDto> Lessons { get; set; } = [];
    }

    public class LessonDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Type { get; set; }
        public int SortOrder { get; set; }
        public int? DurationSeconds { get; set; }
        public bool IsFreePreview { get; set; }
    }

    public class CreateSectionDto
    {
        public string Title { get; set; } = string.Empty;
    }

    public class UpdateSectionDto
    {
        public string Title { get; set; } = string.Empty;
    }

    //important-not imortant!
    public class ReorderItemDto
    {
        public int Id { get; set; }
        public int SortOrder { get; set; }
    }
}
