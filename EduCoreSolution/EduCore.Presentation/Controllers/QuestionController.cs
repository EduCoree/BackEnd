using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Quiz.Teacher;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
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
    [Route("api/teacher/quizzes/{quizId}/questions")]
    [Authorize(Roles ="Teacher")]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private string TeacherId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }
        [HttpGet]
        public async Task<IActionResult> GetQuestions(int quizId)
        {
            var result = await _questionService.GetQuestionsByQuizAsync( quizId,TeacherId);
            return Ok(ApiResponse<QuizDetailsDto>.SuccessResult(result, "Questions Retrieved Succesfully"));
        }
        [HttpPost]
        public async Task<IActionResult> CreateQuestion(int quizId, [FromBody] CreateQuestionDto request)
        {

            var result = await _questionService.AddQuestionAsync(quizId,TeacherId, request);
            var response = ApiResponse<QuestionDto>.SuccessResult(result, "Question Created Successfully");

            return CreatedAtAction(
                nameof(GetQuestions),
                new { quizId = quizId },
                response 
            );

        }
        [HttpPut("{questionId}")]
        public async Task<IActionResult> UpdateQuestion( int quizId, int questionId, [FromBody] UpdateQuestionDto request)
        {
            var result = await _questionService.UpdateQuestionAsync( quizId, questionId,TeacherId, request);
            return Ok(ApiResponse<QuestionDto>.SuccessResult(result, "Question Updated Succesfully"));
        }
        [HttpDelete("{questionId}")]
        public async Task<IActionResult> DeleteQuestion( int quizId, int questionId)
        {
            await _questionService.DeleteQuestionAsync( quizId, questionId, TeacherId);
            return Ok(ApiResponse<string>.SuccessResult("Question Deleted Succesfully"));
        }
    }
}
