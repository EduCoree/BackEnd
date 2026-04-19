using EduCore.Shared.Common;
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
        Task<QuizSummaryDto?> GetQuizSummaryAsync(int quizId, string studentId);
        Task<AttemptDto> StartAttemptAsync(int quizId, string studentId);
        Task<AttemptResultDto> SubmitAttemptAsync(int quizId, int attemptId, string studentId, SubmitAnswerDto request);
        Task<AttemptResultDto> GetResultAsync(int quizId, int attemptId, string studentId);
        Task<IEnumerable<AttemptHistoryDto>> GetQuizHistoryAsync(int quizId, string studentId);
        Task<PagedResult<AttemptHistoryDto>> GetHistoryAsync(string studentId,PaginationParams pagination, HistoryFilterDto filter);
        Task<PagedResult<AvailableQuizzesDto>> GetAvailableQuizzesAsync(string studentId,PaginationParams paginationParams,string? courseTitle);
        Task <IEnumerable<string>>  GetAttemptedCourseTitlesAsync(string studentId);
        Task<IEnumerable<string>> GetAvailableCourseTitles (string studentId);

    }
}
