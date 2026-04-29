using EduCore.Domain.Entities.AuthModel;
using EduCore.Domain.Entities.ContentModel;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Domain.Entities.ProgressModel;
using EduCore.Domain.Entities.QuizModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EduCore.Shared.Enums;
namespace EduCore.Domain.Entities.CourseModel
{
    public class Course:BaseEntity<int>
    {
        public int CategoryId { get; set; }
        public string TeacherId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? CoverImage { get; set; }
        public CourseLevel Level { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public CoursePricingType PricingType { get; set; }
        public CourseStatus Status { get; set; } = CourseStatus.Draft;
        public DateTime CreatedAt { get; set; }

        // Navigation — Parents
        public Category Category { get; set; } = null!;
        public User Teacher { get; set; } = null!;

        // Navigation — Children
        public ICollection<Section> Sections { get; set; } = new List<Section>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<LiveSession> LiveSessions { get; set; } = new List<LiveSession>();
        public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
        public ICollection<CourseReview> Reviews { get; set; } = new List<CourseReview>();
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    }
}
