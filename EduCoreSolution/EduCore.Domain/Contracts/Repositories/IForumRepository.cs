using EduCore.Domain.Entities.ForumModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EduCore.Domain.Contracts.Repositories
{
    public interface IForumRepository
    {
        Task<IEnumerable<ForumPost>> GetPostsByLessonAsync(int lessonId, string? sort);
        Task<ForumPost?> GetPostWithDetailsAsync(int postId);
        Task<ForumReply?> GetReplyWithUserAsync(int replyId);
        Task<IEnumerable<PostReport>> GetAllReportsWithDetailsAsync();
        Task<int> GetReportCountAsync(int postId);
        Task<IEnumerable<string>> GetAdminUserIdsAsync();
    }
}
