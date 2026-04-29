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
    [Route("api/student/my-reviews")]
    [Authorize(Roles = "Student")]
    public class StudentReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public StudentReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        private string StudentId =>
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("Student ID not found");

        // GET api/student/my-reviews
        [HttpGet]
        public async Task<IActionResult> GetMyReviews()
        {
            var reviews = await _reviewService.GetStudentReviewsAsync(StudentId);

            return Ok(ApiResponse<IEnumerable<ReviewDto>>.SuccessResult(reviews));
        }
    }
}

