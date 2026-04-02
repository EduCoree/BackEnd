using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Centers
{
   public class CreateCenterDto
    {
        [Required, MaxLength(120)]
        public string Name { get; set; } = null!;

        [MaxLength(255)]
        public string? LogoUrl { get; set; }

        [Required, EmailAddress, MaxLength(150)]
        public string ContactEmail { get; set; } = null!;

        [MaxLength(20)]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        public SocialLinksDto? SocialLinks { get; set; }
    }
}
