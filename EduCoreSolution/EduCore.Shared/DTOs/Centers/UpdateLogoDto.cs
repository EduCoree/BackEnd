using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Centers
{
    public class UpdateLogoDto
    {
        [Required]
        [Url]
        public string LogoUrl { get; set; } = null!;
    }
}
