using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Identity
{
    public class sendOtpDto
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public OtpPurpose Purpose { get; set; }
    }
}
