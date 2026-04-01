using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.CourseDTOs
{
    public class UpdatePricingDto
    {
        [Required]
        public string PricingType { get; set; } = string.Empty; 
        public decimal Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
    }
}
