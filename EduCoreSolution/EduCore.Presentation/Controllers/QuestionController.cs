using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Quiz;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/teacher/courses/{courseId}/quizzes/{quizId}/questions")]
    public class QuestionController:ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }
        [HttpGet]
        public async Task<IActionResult> GetQuestions(int courseId, int quizId)
        {
            var result = await _questionService.GetQuestionsByQuizAsync(courseId, quizId);
            return Ok(ApiResponse<QuizDetailsDto>.SuccessResult(result,"Questions Retrieved Succesfully"));
        }
        [HttpPost]
        public async Task<IActionResult> CreateQuestion(int courseId, int quizId, [FromBody] CreateQuestionDto request)
        {
            var result = await _questionService.AddQuestionAsync(courseId, quizId, request);
            return CreatedAtAction(nameof(GetQuestions), new { courseId = courseId, quizId = quizId }, result);
        }

    }
}
