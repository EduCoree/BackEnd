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
    [Route("api/teacher/courses/{courseId}/quizzes")]
    public class TeacherQuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public TeacherQuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateQuiz(int courseId, [FromBody] CreateQuizDto request)
        {
           var result = await _quizService.CreateQuizAsync(courseId, request);
           return CreatedAtAction(nameof(GetQuizById), new { courseId=courseId, quizId = result.Id }, ApiResponse<QuizDto>.SuccessResult(result, "Quiz created successfully."));
        }
        [HttpGet("{quizId}")]

        public async Task<IActionResult> GetQuizById(int courseId, int quizId)
        {
            
           var result = await _quizService.GetQuizByIdAsync(courseId, quizId);
           return Ok(ApiResponse<QuizDto>.SuccessResult(result, "Quiz retrieved successfully."));
        }

        [HttpGet]
        public async Task<IActionResult> GetQuizzesByCourse(int courseId)
        {
           
           var result = await _quizService.GetQuizzesByCourseAsync(courseId);
           return Ok(ApiResponse<IEnumerable<QuizDto>>.SuccessResult(result, "Quizzes retrieved successfully."));
          
        }

        [HttpPut("{quizId}")]
        public async Task<IActionResult> Updatequiz(int CourseId, int quizId, [FromBody] CreateQuizDto request)
        {

            var result = await _quizService.UpdateQuizAsync(CourseId,quizId, request);
            return Ok(ApiResponse<QuizDto>.SuccessResult(result, "Quiz updated successfully."));
        }

        [HttpDelete("{quizId}")]
        public async Task<IActionResult> DeleteQuiz(int courseId, int quizId)
        {
            await _quizService.DeleteQuizAsync(courseId, quizId);
            return Ok(ApiResponse<string>.SuccessResult("Quiz deleted successfully.", "Quiz deleted successfully."));
        }
    }


}
