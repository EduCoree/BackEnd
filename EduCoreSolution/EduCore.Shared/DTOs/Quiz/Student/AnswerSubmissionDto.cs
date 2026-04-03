using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz.Student
{
    public class AnswerSubmissionDto
    {
        [Required]
        public int QuestionId { get; init; }
        [Required]
        public int AnswerOptionId { get; init; }
    }
}
