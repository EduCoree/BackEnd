using EduCore.Shared.DTOs.Quiz.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface IstudentQuizService
    {
        Task<StudentQuizDto> GetQuizAsync(int quizId, string studentId);
        Task<AttemptDto> StartAttemptAsync(int quizId, string studentId);
    }
}
