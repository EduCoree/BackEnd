using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Centers
{
    public class CenterDto
    {
       
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? LogoUrl { get; set; }
        public string ContactEmail { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public SocialLinksDto? SocialLinks { get; set; }  
        public DateTime CreatedAt { get; set; }
    }
}

