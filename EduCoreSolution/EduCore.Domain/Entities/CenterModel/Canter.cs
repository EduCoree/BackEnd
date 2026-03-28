using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.CourseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.CenterModel
{
    
        public class Center : BaseEntity<int>
        {
             
            public string Name { get; set; } = null!;
            public string? LogoUrl { get; set; }
            public string ContactEmail { get; set; } = null!;
            public string? Phone { get; set; }
            public string? Address { get; set; }
            public string? SocialLinks { get; set; }   // stored as JSON string
            public DateTime CreatedAt { get; set; }

            // Navigation
            public ICollection<User> Users { get; set; } = new List<User>();
            public ICollection<Category> Categories { get; set; } = new List<Category>();
        }

    
}
