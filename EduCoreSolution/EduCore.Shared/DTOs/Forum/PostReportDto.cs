using System;

namespace EduCore.Shared.DTOs.Forum
{
    public class PostReportDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string PostTitle { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
