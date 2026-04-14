using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Quiz.Student;
using EduCore.Shared.DTOs.Quiz.Teacher;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/teacher/questions/{questionId}/options")]
public class AnswerOptionsController : ControllerBase
{
    private readonly IAnswerOptionService _answerOptionService;
    private string TeacherId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public AnswerOptionsController(IAnswerOptionService answerOptionService)
        => _answerOptionService = answerOptionService;

    [HttpPost]
    public async Task<IActionResult> AddAnswerOption( int questionId,
        [FromBody] CreateAnswerOptionDto request)
    {
        var result = await _answerOptionService.AddAnswerOptionAsync( questionId,TeacherId, request);
        return CreatedAtAction(nameof(AddAnswerOption), new { questionId },
            ApiResponse<AnswerOptionDto>.SuccessResult(result, "Answer option added successfully."));
    }

    [HttpPut("{optionId}")]
    public async Task<IActionResult> UpdateAnswerOption(
        int questionId, int optionId,
        [FromBody] UpdateAnswerOptionDto request)
    {
        var result = await _answerOptionService.UpdateAnswerOptionAsync( questionId, optionId,TeacherId, request);
        return Ok(ApiResponse<AnswerOptionDto>.SuccessResult(result, "Answer option updated successfully."));
    }

    [HttpDelete("{optionId}")]
    public async Task<IActionResult> DeleteAnswerOption(
         int questionId, int optionId)
    {
        await _answerOptionService.DeleteAnswerOptionAsync(questionId, optionId, TeacherId);
        return NoContent();
    }
}