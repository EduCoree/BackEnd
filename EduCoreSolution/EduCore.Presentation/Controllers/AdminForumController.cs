using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Forum;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/forum")]
    [Authorize(Roles = "Admin")]
    public class AdminForumController : ControllerBase
    {
        private readonly IForumService _forumService;

        public AdminForumController(IForumService forumService)
        {
            _forumService = forumService;
        }

        private string AdminUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // GET /api/admin/forum/reports
        [HttpGet("reports")]
        public async Task<IActionResult> GetReports()
        {
            var result = await _forumService.GetAllReportsAsync();
            return Ok(ApiResponse<IEnumerable<PostReportDto>>.SuccessResult(result, "Reports retrieved successfully."));
        }

        // PUT /api/admin/forum/reports/{reportId}/dismiss
        [HttpPut("reports/{reportId:int}/dismiss")]
        public async Task<IActionResult> DismissReport(int reportId)
        {
            await _forumService.DismissReportAsync(reportId);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Report dismissed successfully."));
        }

        // DELETE /api/admin/forum/posts/{postId}
        [HttpDelete("posts/{postId:int}")]
        public async Task<IActionResult> DeletePost(int postId)
        {
            await _forumService.AdminDeletePostAsync(postId, AdminUserId);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Post removed by admin."));
        }
    }
}
