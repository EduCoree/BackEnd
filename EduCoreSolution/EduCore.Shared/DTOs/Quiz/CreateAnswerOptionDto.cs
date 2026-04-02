using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz
{
    public class CreateAnswerOptionDto
    {
        [Required]
        public string Text { get; init; } = string.Empty;
        public bool IsCorrect { get; init; }
    }
}
