using EduCore.Domain.Entities.AuthModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.Common;
using EduCore.Shared.DTOs.PayoutDTOs;
using EduCore.Shared.Enums;
using EduCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/teacher/payout")]
    [Authorize(Roles = "Teacher")]
    public class TeacherPayoutController : ControllerBase
    {
        private readonly ITeacherPayoutService _payoutService;
        private readonly IPayoutSettingsService _settingsService;

        public TeacherPayoutController(
            ITeacherPayoutService payoutService,
            IPayoutSettingsService settingsService)
        {
            _payoutService = payoutService;
            _settingsService = settingsService;
        }

        private string TeacherId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // ─── Earnings ──────────────────────────────────────────────────

        /// <summary>
        /// Paged list of my earnings. Optional date range filter.
        /// </summary>
        [HttpGet("earnings")]
        public async Task<IActionResult> GetMyEarnings(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] PaginationParams pagination)
        {
            var result = await _payoutService.GetMyEarningsAsync(TeacherId, from, to, pagination);
            return Ok(ApiResponse<PagedResult<TeacherEarningDto>>.SuccessResult(result));
        }

        /// <summary>
        /// Real-time preview: how much have I earned this month so far + next tier progress.
        /// </summary>
        [HttpGet("earnings/current-month")]
        public async Task<IActionResult> GetCurrentMonthPreview()
        {
            var result = await _payoutService.GetCurrentMonthPreviewAsync(TeacherId);
            return Ok(ApiResponse<CurrentMonthEarningsDto>.SuccessResult(result));
        }

        /// <summary>
        /// Lifetime summary (total earned, paid, pending, invoice count).
        /// </summary>
        [HttpGet("earnings/summary")]
        public async Task<IActionResult> GetEarningsSummary()
        {
            var result = await _payoutService.GetEarningsSummaryAsync(TeacherId);
            return Ok(ApiResponse<TeacherEarningsSummaryDto>.SuccessResult(result));
        }

        // ─── Invoices ──────────────────────────────────────────────────

        /// <summary>
        /// Paged list of my invoices. Optional status filter (Issued / Paid / Cancelled).
        /// </summary>
        [HttpGet("invoices")]
        public async Task<IActionResult> GetMyInvoices(
            [FromQuery] InvoiceStatus? status,
            [FromQuery] PaginationParams pagination)
        {
            var result = await _payoutService.GetMyInvoicesAsync(TeacherId, status, pagination);
            return Ok(ApiResponse<PagedResult<TeacherInvoiceDto>>.SuccessResult(result));
        }

        /// <summary>
        /// Full detail of one invoice (with all earning lines).
        /// </summary>
        [HttpGet("invoices/{invoiceId:int}")]
        public async Task<IActionResult> GetInvoiceDetails(int invoiceId)
        {
            var result = await _payoutService.GetInvoiceDetailsAsync(invoiceId, TeacherId);
            return Ok(ApiResponse<TeacherInvoiceDetailDto>.SuccessResult(result));
        }

        // ─── Info ──────────────────────────────────────────────────────

        /// <summary>
        /// Current payout rules (commission rate + tier bonuses).
        /// Teachers read this to understand how their earnings are calculated.
        /// </summary>
        [HttpGet("settings")]
        public async Task<IActionResult> GetPayoutSettings()
        {
            var result = await _settingsService.GetSettingsAsync();
            return Ok(ApiResponse<PayoutSettingsDto>.SuccessResult(result));
        }
    }
}
