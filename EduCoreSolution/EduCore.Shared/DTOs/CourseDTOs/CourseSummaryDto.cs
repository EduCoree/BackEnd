using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.CourseDTOs
{
    public class CourseSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? CoverImage { get; set; }
        public CourseLevel Level { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public CoursePricingType PricingType { get; set; }
        public CourseStatus Status { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
    }
}
