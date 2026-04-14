using System;
using System.Collections.Generic;

namespace EduCore.Shared.DTOs.Forum
{
    public class ForumPostDetailDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string StudentId { get; set; } = null!;
        public string StudentName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public int UpvoteCount { get; set; }
        public bool IsRemoved { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ForumReplyDto> Replies { get; set; } = new();
    }
}
