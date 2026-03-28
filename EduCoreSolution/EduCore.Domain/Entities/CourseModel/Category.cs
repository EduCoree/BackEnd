using EduCore.Domain.Entities.CenterModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.CourseModel
{
    public class Category: BaseEntity<int>
    {
    
        public int CenterId { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Center Center { get; set; } = null!;
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
