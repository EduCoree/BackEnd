using System;

namespace EduCore.Shared.DTOs.Forum
{
    public class ForumReplyDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Body { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
