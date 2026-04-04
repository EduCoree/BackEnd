using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Identity
{
    public class EmailConfirmationDto
    {
        public string Email { get; set; }= string.Empty;
        public string Token { get; set; }= string.Empty;
    }
}
