using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.CourseModel
{
    public class Section: BaseEntity<int>
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Course Course { get; set; } = null!;
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
