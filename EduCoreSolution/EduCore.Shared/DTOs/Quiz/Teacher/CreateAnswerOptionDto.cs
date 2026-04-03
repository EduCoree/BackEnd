using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz.Teacher
{
    public class CreateAnswerOptionDto
    {
        [Required]
        [MaxLength(500)]
        public string Text { get; init; } = string.Empty;
        public bool IsCorrect { get; init; }
    }
}
