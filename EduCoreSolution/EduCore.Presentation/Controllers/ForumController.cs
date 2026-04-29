using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Forum;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/lessons/{lessonId:int}/forum")]
    [Authorize]
    public class ForumController : ControllerBase
    {
        private readonly IForumService _forumService;

        public ForumController(IForumService forumService)
        {
            _forumService = forumService;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // GET /api/lessons/{lessonId}/forum/posts?sort=newest|most_upvoted
        [HttpGet("posts")]
        [Authorize(Roles = "Student,Teacher")]
        public async Task<IActionResult> GetPosts(int lessonId, [FromQuery] string? sort)
        {
            var result = await _forumService.GetPostsAsync(lessonId, sort);
            return Ok(ApiResponse<IEnumerable<ForumPostDto>>.SuccessResult(result, "Posts retrieved successfully."));
        }

        // GET /api/lessons/{lessonId}/forum/posts/{postId}
        [HttpGet("posts/{postId:int}")]
        [Authorize(Roles = "Student,Teacher")]
        public async Task<IActionResult> GetPostById(int lessonId, int postId)
        {
            var result = await _forumService.GetPostByIdAsync(postId);
            return Ok(ApiResponse<ForumPostDetailDto>.SuccessResult(result, "Post retrieved successfully."));
        }

        // POST /api/lessons/{lessonId}/forum/posts
        [HttpPost("posts")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CreatePost(int lessonId, [FromBody] CreateForumPostDto dto)
        {
            var result = await _forumService.CreatePostAsync(lessonId, UserId, dto);
            return CreatedAtAction(nameof(GetPosts), new { lessonId },
                ApiResponse<ForumPostDto>.SuccessResult(result, "Post created successfully."));
        }

        // PUT /api/lessons/{lessonId}/forum/posts/{postId}
        [HttpPut("posts/{postId:int}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UpdatePost(int lessonId, int postId, [FromBody] UpdateForumPostDto dto)
        {
            var result = await _forumService.UpdatePostAsync(postId, UserId, dto);
            return Ok(ApiResponse<ForumPostDto>.SuccessResult(result, "Post updated successfully."));
        }

        // DELETE /api/lessons/{lessonId}/forum/posts/{postId}
        [HttpDelete("posts/{postId:int}")]
        [Authorize(Roles = "Student,Admin")]
        public async Task<IActionResult> DeletePost(int lessonId, int postId)
        {
            var isAdmin = User.IsInRole("Admin");
            await _forumService.DeletePostAsync(postId, UserId, isAdmin);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Post deleted successfully."));
        }

        // POST /api/lessons/{lessonId}/forum/posts/{postId}/replies
        [HttpPost("posts/{postId:int}/replies")]
        [Authorize(Roles = "Student,Teacher")]
        public async Task<IActionResult> CreateReply(int lessonId, int postId, [FromBody] CreateForumReplyDto dto)
        {
            var result = await _forumService.CreateReplyAsync(postId, UserId, dto);
            return CreatedAtAction(nameof(GetPosts), new { lessonId },
                ApiResponse<ForumReplyDto>.SuccessResult(result, "Reply created successfully."));
        }

        // PUT /api/lessons/{lessonId}/forum/posts/{postId}/replies/{replyId}
        [HttpPut("posts/{postId:int}/replies/{replyId:int}")]
        [Authorize(Roles = "Student,Teacher")]
        public async Task<IActionResult> UpdateReply(int lessonId, int postId, int replyId, [FromBody] UpdateForumReplyDto dto)
        {
            var result = await _forumService.UpdateReplyAsync(postId, replyId, UserId, dto);
            return Ok(ApiResponse<ForumReplyDto>.SuccessResult(result, "Reply updated successfully."));
        }

        // DELETE /api/lessons/{lessonId}/forum/posts/{postId}/replies/{replyId}
        [HttpDelete("posts/{postId:int}/replies/{replyId:int}")]
        [Authorize(Roles = "Student,Teacher,Admin")]
        public async Task<IActionResult> DeleteReply(int lessonId, int postId, int replyId)
        {
            var isAdmin = User.IsInRole("Admin");
            await _forumService.DeleteReplyAsync(postId, replyId, UserId, isAdmin);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Reply deleted successfully."));
        }

        // POST /api/lessons/{lessonId}/forum/posts/{postId}/upvote
        [HttpPost("posts/{postId:int}/upvote")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UpvotePost(int lessonId, int postId)
        {
            await _forumService.UpvotePostAsync(postId, UserId);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Post upvoted successfully."));
        }

        // POST /api/lessons/{lessonId}/forum/posts/{postId}/report
        [HttpPost("posts/{postId:int}/report")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> ReportPost(int lessonId, int postId, [FromBody] CreatePostReportDto dto)
        {
            var result = await _forumService.ReportPostAsync(postId, UserId, dto);
            return CreatedAtAction(nameof(GetPosts), new { lessonId },
                ApiResponse<PostReportDto>.SuccessResult(result, "Post reported successfully."));
        }
    }
}

