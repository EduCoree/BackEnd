namespace EduCore.Domain.Entities.ChatModel
{
    public class ChatMessage : BaseEntity<int>
    {
        public string UserId { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
