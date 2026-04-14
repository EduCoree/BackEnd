using System;

namespace EduCore.Shared.DTOs.Progress
{
    public class CertificateResponse
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = null!;
        public string StudentName { get; set; } = null!;
        public DateTime IssuedAt { get; set; }
        public string CertificateUuid { get; set; } = null!;
        public string CertificateUrl { get; set; } = null!;
    }
}
