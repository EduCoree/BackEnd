using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.EnrollmentDTOs
{
    public class CashPaymentDto
    {
        [Required]
        public string StudentId { get; set; } = string.Empty;

        [Required]
        public int CourseId { get; set; }

        [Required]
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
    }
}
