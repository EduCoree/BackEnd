using AutoMapper;
using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Domain.Entities.ForumModel;
using EduCore.Domain.Entities.NotificationsModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Forum;
using EduCore.Shared.Enums;
using EduCore.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class ForumService : IForumService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ForumService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Posts

        public async Task<IEnumerable<ForumPostDto>> GetPostsAsync(int courseId, string? sort)
        {
            var posts = await _unitOfWork.ForumRepository.GetPostsByCourseAsync(courseId, sort);
            return _mapper.Map<IEnumerable<ForumPostDto>>(posts);
        }

        public async Task<ForumPostDto> CreatePostAsync(int courseId, string studentId, CreateForumPostDto dto)
        {
            // Check active enrollment
            var isEnrolled = await _unitOfWork.GetRepository<Enrollment, int>()
                .AnyAsync(e => e.StudentId == studentId
                            && e.CourseId == courseId
                            && e.Status == EnrollmentStatus.Active);

            if (!isEnrolled)
                throw new BadRequestException("You must be enrolled in this course to create a post");

            var post = new ForumPost
            {
                CourseId = courseId,
                StudentId = studentId,
                Title = dto.Title,
                Body = dto.Body,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<ForumPost, int>().AddAsync(post);
            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.ForumRepository.GetPostWithDetailsAsync(post.Id);
            return _mapper.Map<ForumPostDto>(created);
        }

        public async Task<ForumPostDto> UpdatePostAsync(int postId, string userId, UpdateForumPostDto dto)
        {
            var post = await GetActivePostOrThrowAsync(postId);

            if (post.StudentId != userId)
                throw new UnauthorizedException("You can only edit your own posts");

            post.Title = dto.Title;
            post.Body = dto.Body;

            _unitOfWork.GetRepository<ForumPost, int>().Update(post);
            await _unitOfWork.SaveChangesAsync();

            var updated = await _unitOfWork.ForumRepository.GetPostWithDetailsAsync(post.Id);
            return _mapper.Map<ForumPostDto>(updated);
        }

        public async Task DeletePostAsync(int postId, string userId, bool isAdmin)
        {
            var post = await GetActivePostOrThrowAsync(postId);

            if (!isAdmin && post.StudentId != userId)
                throw new UnauthorizedException("You can only delete your own posts");

            post.IsRemoved = true;
            _unitOfWork.GetRepository<ForumPost, int>().Update(post);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region Replies

        public async Task<ForumReplyDto> CreateReplyAsync(int postId, string userId, CreateForumReplyDto dto)
        {
            var post = await GetActivePostOrThrowAsync(postId);

            var reply = new ForumReply
            {
                PostId = postId,
                UserId = userId,
                Body = dto.Body,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<ForumReply, int>().AddAsync(reply);

            // Notify post author (if replier is not the author)
            if (post.StudentId != userId)
            {
                var notification = new Notification
                {
                    UserId = post.StudentId,
                    Type = "forum_reply",
                    Title = "New reply on your post",
                    Message = $"Someone replied to your post \"{post.Title}\"",
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.GetRepository<Notification, int>().AddAsync(notification);
            }

            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.ForumRepository.GetReplyWithUserAsync(reply.Id);
            return _mapper.Map<ForumReplyDto>(created);
        }

        public async Task<ForumReplyDto> UpdateReplyAsync(int postId, int replyId, string userId, UpdateForumReplyDto dto)
        {
            await GetActivePostOrThrowAsync(postId);

            var reply = await _unitOfWork.ForumRepository.GetReplyWithUserAsync(replyId);

            if (reply == null || reply.IsRemoved)
                throw new NotFoundException("Reply not found");

            if (reply.PostId != postId)
                throw new BadRequestException("Reply does not belong to this post");

            if (reply.UserId != userId)
                throw new UnauthorizedException("You can only edit your own replies");

            reply.Body = dto.Body;
            _unitOfWork.GetRepository<ForumReply, int>().Update(reply);
            await _unitOfWork.SaveChangesAsync();

            var updated = await _unitOfWork.ForumRepository.GetReplyWithUserAsync(replyId);
            return _mapper.Map<ForumReplyDto>(updated);
        }

        public async Task DeleteReplyAsync(int postId, int replyId, string userId, bool isAdmin)
        {
            var reply = await _unitOfWork.GetRepository<ForumReply, int>().GetByIdAsync(replyId);

            if (reply == null || reply.IsRemoved)
                throw new NotFoundException("Reply not found");

            if (reply.PostId != postId)
                throw new BadRequestException("Reply does not belong to this post");

            if (!isAdmin && reply.UserId != userId)
                throw new UnauthorizedException("You can only delete your own replies");

            reply.IsRemoved = true;
            _unitOfWork.GetRepository<ForumReply, int>().Update(reply);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region Upvotes

        public async Task UpvotePostAsync(int postId, string userId)
        {
            var post = await GetActivePostOrThrowAsync(postId);

            var hasUpvoted = await _unitOfWork.GetRepository<PostUpvote, int>()
                .AnyAsync(u => u.UserId == userId && u.PostId == postId);

            if (hasUpvoted)
                throw new BadRequestException("You have already upvoted this post");

            var upvote = new PostUpvote
            {
                UserId = userId,
                PostId = postId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<PostUpvote, int>().AddAsync(upvote);

            post.UpvoteCount++;
            _unitOfWork.GetRepository<ForumPost, int>().Update(post);

            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region Reports

        public async Task<PostReportDto> ReportPostAsync(int postId, string userId, CreatePostReportDto dto)
        {
            var post = await GetActivePostOrThrowAsync(postId);

            var report = new PostReport
            {
                UserId = userId,
                PostId = postId,
                Reason = dto.Reason,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<PostReport, int>().AddAsync(report);
            await _unitOfWork.SaveChangesAsync();

            // Notify admins if report count >= 3
            var reportCount = await _unitOfWork.ForumRepository.GetReportCountAsync(postId);
            if (reportCount >= 3)
            {
                var adminIds = await _unitOfWork.ForumRepository.GetAdminUserIdsAsync();
                foreach (var adminId in adminIds)
                {
                    var notification = new Notification
                    {
                        UserId = adminId,
                        Type = "post_reported",
                        Title = "Post reported multiple times",
                        Message = $"Post \"{post.Title}\" has been reported {reportCount} times and may need review",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.GetRepository<Notification, int>().AddAsync(notification);
                }
                await _unitOfWork.SaveChangesAsync();
            }

            return new PostReportDto
            {
                Id = report.Id,
                PostId = report.PostId,
                PostTitle = post.Title,
                UserId = report.UserId,
                UserName = post.Student?.Name ?? "",
                Reason = report.Reason,
                CreatedAt = report.CreatedAt
            };
        }

        public async Task<IEnumerable<PostReportDto>> GetAllReportsAsync()
        {
            var reports = await _unitOfWork.ForumRepository.GetAllReportsWithDetailsAsync();
            return _mapper.Map<IEnumerable<PostReportDto>>(reports);
        }

        public async Task DismissReportAsync(int reportId)
        {
            var report = await _unitOfWork.GetRepository<PostReport, int>().GetByIdAsync(reportId);

            if (report == null)
                throw new NotFoundException("Report not found");

            _unitOfWork.GetRepository<PostReport, int>().Remove(report);
            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region Admin

        public async Task AdminDeletePostAsync(int postId, string adminUserId)
        {
            var post = await _unitOfWork.GetRepository<ForumPost, int>().GetByIdAsync(postId);

            if (post == null)
                throw new NotFoundException("Post not found");

            post.IsRemoved = true;
            _unitOfWork.GetRepository<ForumPost, int>().Update(post);

            // Write audit log
            var auditLog = new AuditLog
            {
                UserId = adminUserId,
                Action = "forum_post.delete",
                EntityType = "ForumPost",
                EntityId = postId,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.GetRepository<AuditLog, int>().AddAsync(auditLog);

            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region Helpers

        private async Task<ForumPost> GetActivePostOrThrowAsync(int postId)
        {
            var post = await _unitOfWork.ForumRepository.GetPostWithDetailsAsync(postId);

            if (post == null || post.IsRemoved)
                throw new NotFoundException("Post not found");

            return post;
        }

        #endregion
    }
}
