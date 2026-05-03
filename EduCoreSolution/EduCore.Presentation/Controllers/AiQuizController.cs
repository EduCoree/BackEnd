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
    [Route("api/teacher/quizzes/{quizId}/ai-quiz")]
    [Authorize(Roles = "Teacher")]
    public class AiQuizController:ControllerBase
    {
        private readonly IAiQuizService _aiQuizService;
        private string TeacherId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public AiQuizController(IAiQuizService aiQuizService)
        {
            _aiQuizService = aiQuizService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate(int quizId,[FromBody] GenerateQuizAiRequest request)
        {
            var result = await _aiQuizService.GenerateQuizAsync(quizId, TeacherId, request);
            return Ok(ApiResponse<AiGeneratedQuizDto>.SuccessResult(result, "Quiz generated successfully."));
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save(int quizId,[FromBody] AiGeneratedQuizDto request)
        {
            var result = await _aiQuizService.SaveGeneratedQuizAsync(quizId, TeacherId, request);
            return Ok(ApiResponse<AiGeneratedQuizDto>.SuccessResult(result, "Quiz saved successfully."));
        }
    }
}
