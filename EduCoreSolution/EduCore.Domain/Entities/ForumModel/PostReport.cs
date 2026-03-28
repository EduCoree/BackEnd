using EduCore.Domain.Entities.AuthModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.ForumModel
{
    public class PostReport : BaseEntity<int>
    {
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string Reason { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public User User { get; set; } = null!;
        public ForumPost Post { get; set; } = null!;
    }
}
