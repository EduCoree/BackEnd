using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Identity
{
    public class VerifyOtpResponseDto
    {
        // Common properties (always present)
        public string Message { get; set; }

        // Password reset specific (null for email verification)
        public string? ResetToken { get; set; }
        public int? ExpiresInSeconds { get; set; }

        // Email verification specific (null for password reset)
        public string? ConfirmedEmail { get; set; }
    }
}
