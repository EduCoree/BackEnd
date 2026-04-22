using EduCore.Services_Abstraction;
using EduCore.Shared.Common;
using EduCore.Shared.CommonResult;
using EduCore.Shared.DTOs.Quiz.Teacher;
using EduCore.Shared.Exceptions;
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
    [Route("api/teacher")]
    [Authorize(Roles = "Teacher")]
    public class TeacherQuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private string TeacherId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        public TeacherQuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }
        [HttpPost("courses/{courseId}/quizzes")]
        public async Task<IActionResult> CreateQuiz(int courseId, [FromBody] CreateQuizDto request)
        {
           var result = await _quizService.CreateQuizAsync(courseId,TeacherId, request);
           return CreatedAtAction(nameof(GetQuizById), new { courseId=courseId, quizId = result.Id }, ApiResponse<QuizDto>.SuccessResult(result, "Quiz created successfully."));
        }
        [HttpGet("quizzes/{quizId}")]

        public async Task<IActionResult> GetQuizById(int quizId)
        {
            
           var result = await _quizService.GetQuizByIdAsync( quizId,TeacherId);
           return Ok(ApiResponse<QuizDto>.SuccessResult(result, "Quiz retrieved successfully."));
        }

        [HttpGet("courses/{courseId}/quizzes")]
        public async Task<IActionResult> GetQuizzesByCourse(int courseId,[FromQuery] PaginationParams pagination)
        {
           
           var result = await _quizService.GetQuizzesByCourseAsync(courseId,TeacherId,pagination);
           return Ok(ApiResponse<PagedResult<QuizDto>>.SuccessResult(result, "Quizzes retrieved successfully."));
          
        }

        [HttpPut("quizzes/{quizId}")]
        public async Task<IActionResult> Updatequiz(int quizId, [FromBody] UpdateQuizDto request)
        {

            var result = await _quizService.UpdateQuizAsync(quizId,TeacherId, request);
            return Ok(ApiResponse<QuizDto>.SuccessResult(result, "Quiz updated successfully."));
        }

        [HttpDelete("quizzes/{quizId}")]
        public async Task<IActionResult> DeleteQuiz( int quizId)
        {
            await _quizService.DeleteQuizAsync(quizId, TeacherId);
            return Ok(ApiResponse<string>.SuccessResult("Quiz deleted successfully.", "Quiz deleted successfully."));
        }
        [HttpPost("quizzes/{quizId}/publish")]
        public async Task<IActionResult> PublishQuiz(int quizId)
        {
           var result= await _quizService.PublishQuizAsync(quizId, TeacherId);
            return Ok(ApiResponse<QuizDto>.SuccessResult(result, "Quiz Published successfully."));
        }
    }


}
