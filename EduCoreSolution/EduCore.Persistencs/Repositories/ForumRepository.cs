using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.ForumModel;
using EduCore.Persistencs.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Repositories
{
    public class ForumRepository : IForumRepository
    {
        private readonly EduCoreDbContext _context;

        public ForumRepository(EduCoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ForumPost>> GetPostsByLessonAsync(int lessonId, string? sort)
        {
            var query = _context.ForumPosts
                .Include(p => p.Student)
                .Include(p => p.Replies)
                .Where(p => p.LessonId == lessonId && !p.IsRemoved);

            query = sort?.ToLower() switch
            {
                "most_upvoted" => query.OrderByDescending(p => p.UpvoteCount),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            return await query.ToListAsync();
        }

        public async Task<ForumPost?> GetPostWithDetailsAsync(int postId)
        {
            return await _context.ForumPosts
                .Include(p => p.Student)
                .Include(p => p.Replies.Where(r => !r.IsRemoved))
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == postId);
        }

        public async Task<ForumReply?> GetReplyWithUserAsync(int replyId)
        {
            return await _context.ForumReplies
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == replyId);
        }

        public async Task<IEnumerable<PostReport>> GetAllReportsWithDetailsAsync()
        {
            return await _context.PostReports
                .Include(r => r.User)
                .Include(r => r.Post)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetReportCountAsync(int postId)
        {
            return await _context.PostReports
                .CountAsync(r => r.PostId == postId);
        }

        public async Task<IEnumerable<string>> GetAdminUserIdsAsync()
        {
            var adminRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == "Admin");

            if (adminRole == null) return [];

            return await _context.UserRoles
                .Where(ur => ur.RoleId == adminRole.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();
        }
    }
}
