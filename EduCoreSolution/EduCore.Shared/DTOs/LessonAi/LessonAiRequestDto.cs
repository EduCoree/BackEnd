using System.ComponentModel.DataAnnotations;

namespace EduCore.Shared.DTOs.LessonAi
{
    public class LessonAiRequestDto
    {
        [Required]
        public int LessonId { get; set; }

        /// <summary>
        /// Required for the /ask endpoint. Optional for summarize and translate.
        /// </summary>
        [MaxLength(1000)]
        public string? Question { get; set; }

        /// <summary>
        /// Target language for the /translate endpoint. Defaults to "Arabic" if not provided.
        /// </summary>
        public string? TargetLanguage { get; set; }
    }
}
