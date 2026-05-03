using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz.Teacher
{
    public class GenerateQuizAiRequest
    {
        [Required]
        [MaxLength(500)]
        public string Topic { get; init; }=string.Empty;
        [Range(1, 20)]
        public int QuestionCount { get; init; } = 5;

        public QuestionType QuestionType { get; init; }

        [Range(1, 100)]
        public int PointsPerQuestion { get; init; } = 10;

        public string Difficulty { get; init; } = "Medium";
    }
}
