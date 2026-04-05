using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.AdminUser
{
    public record StudentPaymentDto(
        int Id,
        decimal Amount,
        string Currency,
        string Method,
        string Status,
        string? Reference,
        DateTime? PaidAt
    );
}
