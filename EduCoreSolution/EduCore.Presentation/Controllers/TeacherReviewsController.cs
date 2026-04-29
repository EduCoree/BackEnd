using EduCore.Domain.Entities.AuthModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Reviews;
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
    [Route("api/teacher/reviews")]
    [Authorize(Roles = "Teacher")]
    public class TeacherReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public TeacherReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        private string TeacherId =>
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("Teacher ID not found");

        // GET api/teacher/reviews?courseId=5&minRating=4
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? courseId,
            [FromQuery] int? minRating)
        {
            var reviews = await _reviewService.GetTeacherReviewsAsync(
                TeacherId,
                courseId,
                minRating);

            return Ok(ApiResponse<IEnumerable<ReviewDto>>.SuccessResult(reviews));
        }

        // DELETE api/teacher/reviews/{reviewId}
        [HttpDelete("{reviewId:int}")]
        public async Task<IActionResult> Delete(int reviewId)
        {
            await _reviewService.DeleteReviewByTeacherAsync(TeacherId, reviewId);

            return Ok(ApiResponse<object>.SuccessResult(null, "Review deleted successfully"));
        }
    }
}

