using EduCore.Services_Abstraction;
using EduCore.Shared.Dtos.Quiz;
using EduCore.Shared.Exceptions;
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
    [Route("api/courses/{courseId}/quizzes")]
    public class QuizController:ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateQuiz(int courseId, [FromBody] CreateQuizDto request)
        {
            try
            {
                var result = await _quizService.CreateQuizAsync(courseId, request);
                return CreatedAtAction(nameof(GetQuizById), new { quizId = result.Id }, ApiResponse<QuizDto>.SuccessResult(result, "Quiz created successfully."));
            }
            catch(NotFoundException ex)
            {
                return NotFound(ApiResponse<QuizDto>.FailResult(ex.Message));
            }
            catch(Exception)
            {
                return StatusCode(500, ApiResponse<QuizDto>.FailResult("An unexpected error occurred."));
            }
        }
        [HttpGet("{quizId}")]

        public async Task<IActionResult> GetQuizById(int courseId ,int quizId)
        {
             try
        {
            var result = await _quizService.GetQuizByIdAsync(quizId);
            return Ok(ApiResponse<QuizDto>.SuccessResult(result, "Quiz retrieved successfully."));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<QuizDto>.FailResult(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<QuizDto>.FailResult("An unexpected error occurred."));
        }
        }
    }
}
