namespace EduCore.Shared.DTOs.Content
{
    public class JoinSessionResponse
    {
        public string RoomName { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string? Title { get; set; }
    }
}
