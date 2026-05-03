using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz.Teacher
{
    public class CreateQuestionDto
    {
        [Required]
        [MaxLength(1000)]
        public string Text { get; init; } = string.Empty;

        [EnumDataType(typeof(QuestionType), ErrorMessage = "Invalid question type.")]
        public QuestionType Type { get; init; }

        [Range(1, 100, ErrorMessage = "Points must be between 1 and 100.")]
        public int Points { get; init; }

        [Required]
        [MinLength(2, ErrorMessage = "A question must have at least 2 answer options.")]
        public List<CreateAnswerOptionDto> AnswerOptions { get; init; } = new();
    }
}
