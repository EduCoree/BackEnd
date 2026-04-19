using EduCore.Services_Abstraction;
using EduCore.Shared.Common;
using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.Quiz.Student;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/quizzes")]
    [Authorize(Roles = "Student")]

    public class StudentQuizzesController : ControllerBase
    {
        private readonly IstudentQuizService _studentQuizService;

        public StudentQuizzesController(IstudentQuizService studentQuizService)
            => _studentQuizService = studentQuizService;

        private string StudentId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet("{quizId}")]
        public async Task<IActionResult> GetQuiz(int quizId)
        {
            var result = await _studentQuizService.GetQuizAsync(quizId, StudentId);
            return Ok(ApiResponse<StudentQuizDto>.SuccessResult(result, "Quiz retrieved successfully."));
        }

        [HttpPost("{quizId}/start")]
        public async Task<IActionResult> StartAttempt(int quizId)
        {
            var result = await _studentQuizService.StartAttemptAsync(quizId, StudentId);
            return CreatedAtAction(nameof(GetResult),
                new { quizId, attemptId = result.Id },
                ApiResponse<AttemptDto>.SuccessResult(result, "Attempt started successfully."));
        }

        [HttpPost("{quizId}/attempts/{attemptId}/submit")]
        public async Task<IActionResult> SubmitAttempt(int quizId, int attemptId, [FromBody] SubmitAnswerDto request)
        {
            var result = await _studentQuizService.SubmitAttemptAsync(quizId, attemptId, StudentId, request);
            return Ok(ApiResponse<AttemptResultDto>.SuccessResult(result, "Quiz submitted successfully."));
        }

        [HttpGet("{quizId}/attempts/{attemptId}/result")]
        public async Task<IActionResult> GetResult(int quizId, int attemptId)
        {
            var result = await _studentQuizService.GetResultAsync(quizId, attemptId, StudentId);
            return Ok(ApiResponse<AttemptResultDto>.SuccessResult(result, "Result retrieved successfully."));
        }
        [HttpGet("{quizId}/attempts")]
        public async Task<IActionResult> GetQuizHistory(int quizId)
        {
            var result = await _studentQuizService.GetQuizHistoryAsync(quizId, StudentId);
            return Ok(ApiResponse<IEnumerable<AttemptHistoryDto>>.SuccessResult(result));
        }
        [HttpGet("{quizId}/summary")]
        public async Task<IActionResult> GetQuizSummary(int quizId)
        {
            var summary = await _studentQuizService.GetQuizSummaryAsync(quizId,StudentId);
            return Ok(ApiResponse<QuizSummaryDto>.SuccessResult(summary));

        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] PaginationParams paginationParams, [FromQuery]HistoryFilterDto filter)
        {
            var result = await _studentQuizService.GetHistoryAsync(StudentId,paginationParams,filter);
            return Ok(ApiResponse<PagedResult<AttemptHistoryDto>>.SuccessResult(result, "History retrieved successfully."));
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableQuizzes([FromQuery] PaginationParams paginationParams,[FromQuery] string? courseTitle)
        { 
            var quizzes = await _studentQuizService.GetAvailableQuizzesAsync(StudentId,paginationParams,courseTitle);
            return Ok(ApiResponse<PagedResult<AvailableQuizzesDto>>.SuccessResult(quizzes));
        }
        [HttpGet("history/courses")]
        public async Task<IActionResult> GetAttemptedCourseTitles()
        {
            var courses = await _studentQuizService.GetAttemptedCourseTitlesAsync(StudentId);
            return Ok(ApiResponse<IEnumerable<string>>.SuccessResult(courses));
        }

        [HttpGet("available/courses")]
        public async Task<IActionResult> GetAvailableCourseTitles()
        {
            var courses = await _studentQuizService.GetAvailableCourseTitles(StudentId);
            return Ok(ApiResponse<IEnumerable<string>>.SuccessResult(courses));
        }
    }
}
