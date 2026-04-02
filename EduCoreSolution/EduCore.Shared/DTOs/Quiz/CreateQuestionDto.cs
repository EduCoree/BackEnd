using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz
{
    public class CreateQuestionDto
    {
        [Required]
        public string Text { get; init; } = string.Empty;
        [Required]
        public string Type { get; init; } = string.Empty; 
        [Range(1, int.MaxValue)]
        public int Points { get; init; }
    }
}
