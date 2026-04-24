using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Identity
{
    public class RegisterResponseDto
    {
        public string Message { get; set; }
        public string Email { get; set; }
        public bool RequiresVerification { get; set; }
    }
}
