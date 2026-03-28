using EduCore.Domain.Entities.AuthModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.ForumModel
{
    public class ForumReply : BaseEntity<int>
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Body { get; set; } = null!;
        public int UpvoteCount { get; set; } = 0;
        public bool IsRemoved { get; set; } = false;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public ForumPost Post { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
