using System;

namespace EduCore.Shared.DTOs.Content
{
    public class SignedUrlResponse
    {
        public string Url { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
