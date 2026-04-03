using EduCore.Shared.DTOs.Quiz;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz.Teacher
{
    public class UpdateQuizDto
    {
        
        [MaxLength(200)]
        public string? Title { get; init; } 
        [Range(1, int.MaxValue, ErrorMessage = "TimeLimitMins must be greater than 0.")]
        public int? TimeLimitMins { get; init; }
        [Range(0, 100, ErrorMessage = "PassScore must be between 0 and 100.")]
        public int? PassScore { get; init; }
        [Range(1, int.MaxValue, ErrorMessage = "MaxAttempts must be greater than 0.")]
        public int? MaxAttempts { get; init; }
        public bool? IsRandomized { get; init; }
    }
}
