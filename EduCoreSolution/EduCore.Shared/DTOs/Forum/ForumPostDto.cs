using System;

namespace EduCore.Shared.DTOs.Forum
{
    public class ForumPostDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string StudentId { get; set; } = null!;
        public string StudentName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public int UpvoteCount { get; set; }
        public int ReplyCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
