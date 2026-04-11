using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Forum;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/courses/{courseId:int}/forum")]
    [Authorize]
    public class ForumController : ControllerBase
    {
        private readonly IForumService _forumService;

        public ForumController(IForumService forumService)
        {
            _forumService = forumService;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // GET /api/courses/{courseId}/forum/posts?sort=newest|most_upvoted
        [HttpGet("posts")]
        [Authorize(Roles = "Student,Teacher")]
        public async Task<IActionResult> GetPosts(int courseId, [FromQuery] string? sort)
        {
            var result = await _forumService.GetPostsAsync(courseId, sort);
            return Ok(ApiResponse<IEnumerable<ForumPostDto>>.SuccessResult(result, "Posts retrieved successfully."));
        }

        // GET /api/courses/{courseId}/forum/posts/{postId}
        [HttpGet("posts/{postId:int}")]
        [Authorize(Roles = "Student,Teacher")]
        public async Task<IActionResult> GetPostById(int courseId, int postId)
        {
            var result = await _forumService.GetPostByIdAsync(postId);
            return Ok(ApiResponse<ForumPostDetailDto>.SuccessResult(result, "Post retrieved successfully."));
        }

        // POST /api/courses/{courseId}/forum/posts
        [HttpPost("posts")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CreatePost(int courseId, [FromBody] CreateForumPostDto dto)
        {
            var result = await _forumService.CreatePostAsync(courseId, UserId, dto);
            return CreatedAtAction(nameof(GetPosts), new { courseId },
                ApiResponse<ForumPostDto>.SuccessResult(result, "Post created successfully."));
        }

        // PUT /api/courses/{courseId}/forum/posts/{postId}
        [HttpPut("posts/{postId:int}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UpdatePost(int courseId, int postId, [FromBody] UpdateForumPostDto dto)
        {
            var result = await _forumService.UpdatePostAsync(postId, UserId, dto);
            return Ok(ApiResponse<ForumPostDto>.SuccessResult(result, "Post updated successfully."));
        }

        // DELETE /api/courses/{courseId}/forum/posts/{postId}
        [HttpDelete("posts/{postId:int}")]
        [Authorize(Roles = "Student,Admin")]
        public async Task<IActionResult> DeletePost(int courseId, int postId)
        {
            var isAdmin = User.IsInRole("Admin");
            await _forumService.DeletePostAsync(postId, UserId, isAdmin);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Post deleted successfully."));
        }

        // POST /api/courses/{courseId}/forum/posts/{postId}/replies
        [HttpPost("posts/{postId:int}/replies")]
        [Authorize(Roles = "Student,Teacher")]
        public async Task<IActionResult> CreateReply(int courseId, int postId, [FromBody] CreateForumReplyDto dto)
        {
            var result = await _forumService.CreateReplyAsync(postId, UserId, dto);
            return CreatedAtAction(nameof(GetPosts), new { courseId },
                ApiResponse<ForumReplyDto>.SuccessResult(result, "Reply created successfully."));
        }

        // PUT /api/courses/{courseId}/forum/posts/{postId}/replies/{replyId}
        [HttpPut("posts/{postId:int}/replies/{replyId:int}")]
        [Authorize(Roles = "Student,Teacher")]
        public async Task<IActionResult> UpdateReply(int courseId, int postId, int replyId, [FromBody] UpdateForumReplyDto dto)
        {
            var result = await _forumService.UpdateReplyAsync(postId, replyId, UserId, dto);
            return Ok(ApiResponse<ForumReplyDto>.SuccessResult(result, "Reply updated successfully."));
        }

        // DELETE /api/courses/{courseId}/forum/posts/{postId}/replies/{replyId}
        [HttpDelete("posts/{postId:int}/replies/{replyId:int}")]
        [Authorize(Roles = "Student,Teacher,Admin")]
        public async Task<IActionResult> DeleteReply(int courseId, int postId, int replyId)
        {
            var isAdmin = User.IsInRole("Admin");
            await _forumService.DeleteReplyAsync(postId, replyId, UserId, isAdmin);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Reply deleted successfully."));
        }

        // POST /api/courses/{courseId}/forum/posts/{postId}/upvote
        [HttpPost("posts/{postId:int}/upvote")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UpvotePost(int courseId, int postId)
        {
            await _forumService.UpvotePostAsync(postId, UserId);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Post upvoted successfully."));
        }

        // POST /api/courses/{courseId}/forum/posts/{postId}/report
        [HttpPost("posts/{postId:int}/report")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> ReportPost(int courseId, int postId, [FromBody] CreatePostReportDto dto)
        {
            var result = await _forumService.ReportPostAsync(postId, UserId, dto);
            return CreatedAtAction(nameof(GetPosts), new { courseId },
                ApiResponse<PostReportDto>.SuccessResult(result, "Post reported successfully."));
        }
    }
}
