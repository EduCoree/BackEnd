using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Quiz.Student;
using EduCore.Shared.Responses;
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
    public class StudentQuizzezController : ControllerBase
    {
        private readonly IstudentQuizService _studentQuizService;
        private string StudentId => "2721eab6-9c64-404e-9911-3850dbefb12f";
        public StudentQuizzezController(IstudentQuizService studentQuizService)
            => _studentQuizService = studentQuizService;


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
            return Ok(ApiResponse<AttemptDto>.SuccessResult(result, "Quiz attempt started successfully."));
        }
    }
}
