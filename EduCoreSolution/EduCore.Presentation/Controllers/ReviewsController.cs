using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Reviews;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/courses/{courseId:int}/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // GET api/courses/5/reviews
        [HttpGet]
        public async Task<IActionResult> GetAll(int courseId)
        {
            var reviews = await _reviewService.GetReviewsByCourseAsync(courseId);
            return Ok(reviews);
        }



        // POST api/courses/5/reviews
        [HttpPost]
        public async Task<IActionResult> Create(int courseId, [FromBody] CreateReviewDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // مؤقت للتجربة
            var studentId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                            ?? "test-student-id";

            var (review, error) = await _reviewService.CreateReviewAsync(courseId, studentId, dto);

            if (error is not null)
                return error.Contains("already") ? Conflict(error) : BadRequest(error);

            return CreatedAtAction(nameof(GetAll), new { courseId }, review);
        }

        // PUT api/courses/5/reviews/3
        [HttpPut("{reviewId:int}")]
        public async Task<IActionResult> Update(int courseId, int reviewId, [FromBody] UpdateReviewDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // مؤقت للتجربة
            var studentId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                            ?? "test-student-id";

            var (review, error) = await _reviewService.UpdateReviewAsync(courseId, reviewId, studentId, dto);

            if (error is not null)
                return error.Contains("own") ? Forbid() : NotFound();

            return Ok(review);
        }



        // DELETE api/courses/5/reviews/3
        [HttpDelete("{reviewId:int}")]
        public async Task<IActionResult> Delete(int courseId, int reviewId)
        {
            // مؤقت للتجربة
            var studentId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                            ?? "test-student-id";

            // مؤقت — هيتعمل من الـ JWT Role لاحقاً
            var isAdmin = User.IsInRole("Admin");

            var (success, error) = await _reviewService.DeleteReviewAsync(courseId, reviewId, studentId, isAdmin);

            if (error == "Review not found.")
                return NotFound(error);

            if (error is not null)
                return Forbid();

            return NoContent();
        }


        // GET api/courses/5/reviews/summary
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(int courseId)
        {
            var summary = await _reviewService.GetReviewSummaryAsync(courseId);
            return Ok(summary);
        }
    }
}
