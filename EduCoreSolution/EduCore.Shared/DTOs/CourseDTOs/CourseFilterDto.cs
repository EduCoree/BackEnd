using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.CourseDTOs
{
    public class CourseFilterDto
    {
        public int? CategoryId { get; set; }
        public string? Level { get; set; }       
        public string? PricingType { get; set; } 
        public string? Search { get; set; }      
    }
}
