using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.CourseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Domain.Entities.ForumModel
{
    public class ForumPost : BaseEntity<int>
    {
        public int CourseId { get; set; }
        public string StudentId { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public int UpvoteCount { get; set; } = 0;
        public bool IsRemoved { get; set; } = false;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Course Course { get; set; } = null!;
        public User Student { get; set; } = null!;
        public ICollection<ForumReply> Replies { get; set; } = new List<ForumReply>();
        public ICollection<PostUpvote> Upvotes { get; set; } = new List<PostUpvote>();
        public ICollection<PostReport> Reports { get; set; } = new List<PostReport>();
    }
}
