using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EduCore.Shared.Enums;
namespace EduCore.Shared.DTOs.CourseDTOs
{
    public class CreateCourseDto
    {
        [Required, MaxLength(160)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public CourseLevel? Level { get; set; }

        [Required]
        public decimal Price { get; set; }

        public decimal? DiscountedPrice { get; set; }

        [Required]
        public CoursePricingType? PricingType { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
