using System.ComponentModel.DataAnnotations;

namespace EduCore.Shared.DTOs.Chat
{
    public class ChatRequestDto
    {
        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = null!;

        public int? CourseId { get; set; }
    }
}
