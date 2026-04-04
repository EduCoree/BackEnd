using EduCore.Shared.DTOs.Forum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    public interface IForumService
    {
        // Posts
        Task<IEnumerable<ForumPostDto>> GetPostsAsync(int courseId, string? sort);
        Task<ForumPostDto> CreatePostAsync(int courseId, string studentId, CreateForumPostDto dto);
        Task<ForumPostDto> UpdatePostAsync(int postId, string userId, UpdateForumPostDto dto);
        Task DeletePostAsync(int postId, string userId, bool isAdmin);

        // Replies
        Task<ForumReplyDto> CreateReplyAsync(int postId, string userId, CreateForumReplyDto dto);
        Task<ForumReplyDto> UpdateReplyAsync(int postId, int replyId, string userId, UpdateForumReplyDto dto);
        Task DeleteReplyAsync(int postId, int replyId, string userId, bool isAdmin);

        // Upvotes
        Task UpvotePostAsync(int postId, string userId);

        // Reports
        Task<PostReportDto> ReportPostAsync(int postId, string userId, CreatePostReportDto dto);
        Task<IEnumerable<PostReportDto>> GetAllReportsAsync();
        Task DismissReportAsync(int reportId);

        // Admin
        Task AdminDeletePostAsync(int postId, string adminUserId);
    }
}
