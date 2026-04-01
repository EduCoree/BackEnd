using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EduCore.Shared.Enums;
namespace EduCore.Shared.DTOs.CourseDTOs
{
    public class CourseFilterDto
    {
        public int? CategoryId { get; set; }
        public CourseLevel? Level { get; set; }
        public CoursePricingType? PricingType { get; set; }
        public string? Search { get; set; }      
    }
}
