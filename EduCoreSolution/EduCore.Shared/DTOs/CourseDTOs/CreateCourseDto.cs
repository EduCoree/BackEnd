using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.CourseDTOs
{
    public class CreateCourseDto
    {
        [Required, MaxLength(160)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public string Level { get; set; } = string.Empty; 

        [Required]
        public decimal Price { get; set; }

        public decimal? DiscountedPrice { get; set; }

        [Required]
        public string PricingType { get; set; } = string.Empty; 

        [Required]
        public int CategoryId { get; set; }
    }
}
