using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz.Student
{
    public class SubmitAnswerDto
    {
        [Required]
        public ICollection<AnswerSubmissionDto> Answers { get; init; } = new List<AnswerSubmissionDto>();
    }
}
