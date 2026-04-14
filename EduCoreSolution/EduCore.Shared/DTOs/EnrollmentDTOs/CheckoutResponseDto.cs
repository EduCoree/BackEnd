using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.EnrollmentDTOs
{
    public class CheckoutResponseDto
    {
        public string CheckoutUrl { get; set; } = string.Empty;
        public int PaymentId { get; set; }
    }
}
