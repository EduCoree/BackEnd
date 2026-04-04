using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Identity
{
    public class VerifyOtpDto
    {
        public string Email { get; init; } = string.Empty;
        public string Otp { get; init; } = string.Empty;
    }
}
