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
    [Route("api/admin/payout")]
    [Authorize(Roles = "Admin")]
    public class AdminPayoutController : ControllerBase
    {
        private readonly IAdminPayoutService _adminPayoutService;
        private readonly IPayoutSettingsService _settingsService;

        public AdminPayoutController(
            IAdminPayoutService adminPayoutService,
            IPayoutSettingsService settingsService)
        {
            _adminPayoutService = adminPayoutService;
            _settingsService = settingsService;
        }

        private string AdminId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // ─── Dashboard ─────────────────────────────────────────────────

        /// <summary>
        /// Admin payout dashboard KPIs (pending payouts, current month, last month).
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _adminPayoutService.GetDashboardAsync();
            return Ok(ApiResponse<AdminPayoutDashboardDto>.SuccessResult(result));
        }

        // ─── Invoices ──────────────────────────────────────────────────

        /// <summary>
        /// Paged list of all invoices across all teachers. Multiple filters.
        /// </summary>
        [HttpGet("invoices")]
        public async Task<IActionResult> GetAllInvoices(
            [FromQuery] InvoiceStatus? status,
            [FromQuery] string? teacherId,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] PaginationParams pagination)
        {
            var result = await _adminPayoutService.GetAllInvoicesAsync(
                status, teacherId, from, to, pagination);
            return Ok(ApiResponse<PagedResult<TeacherInvoiceDto>>.SuccessResult(result));
        }

        /// <summary>
        /// Full detail of one invoice (with all earning lines).
        /// </summary>
        [HttpGet("invoices/{invoiceId:int}")]
        public async Task<IActionResult> GetInvoiceDetails(int invoiceId)
        {
            var result = await _adminPayoutService.GetInvoiceDetailsAsync(invoiceId);
            return Ok(ApiResponse<TeacherInvoiceDetailDto>.SuccessResult(result));
        }

        /// <summary>
        /// Mark an invoice as paid (admin disbursed the payout to the teacher).
        /// </summary>
        [HttpPut("invoices/{invoiceId:int}/mark-paid")]
        public async Task<IActionResult> MarkInvoiceAsPaid(
            int invoiceId,
            [FromBody] MarkInvoiceAsPaidDto dto)
        {
            var result = await _adminPayoutService.MarkInvoiceAsPaidAsync(invoiceId, dto, AdminId);
            return Ok(ApiResponse<TeacherInvoiceDto>.SuccessResult(result, "Invoice marked as paid."));
        }

        /// <summary>
        /// Cancel an invoice (data error, wrong calculation, etc).
        /// Restores earnings back to Available status.
        /// </summary>
        [HttpPut("invoices/{invoiceId:int}/cancel")]
        public async Task<IActionResult> CancelInvoice(
            int invoiceId,
            [FromBody] CancelInvoiceDto dto)
        {
            var result = await _adminPayoutService.CancelInvoiceAsync(invoiceId, dto, AdminId);
            return Ok(ApiResponse<TeacherInvoiceDto>.SuccessResult(result, "Invoice cancelled."));
        }

        // ─── Manual Job Trigger ────────────────────────────────────────

        /// <summary>
        /// Manually trigger invoice generation for a specific month.
        /// Useful for:
        ///   - Testing without waiting for the cron schedule
        ///   - Backfilling a month the cron job missed
        ///   - Regenerating after cancelling invoices
        /// Idempotent: teachers who already have an invoice for the period are skipped.
        /// </summary>
        [HttpPost("invoices/generate")]
        public async Task<IActionResult> TriggerInvoiceGeneration(
            [FromQuery] int year,
            [FromQuery] int month)
        {
            var result = await _adminPayoutService.TriggerInvoiceGenerationAsync(year, month);
            return Ok(ApiResponse<GenerateInvoicesResultDto>.SuccessResult(
                result,
                $"Generated {result.InvoicesCreated} invoices for {year}-{month:D2}."));
        }

        // ─── Settings ──────────────────────────────────────────────────

        /// <summary>
        /// Current payout rules (commission rate + tier bonuses).
        /// </summary>
        [HttpGet("settings")]
        public async Task<IActionResult> GetPayoutSettings()
        {
            var result = await _settingsService.GetSettingsAsync();
            return Ok(ApiResponse<PayoutSettingsDto>.SuccessResult(result));
        }

        /// <summary>
        /// Update payout rules.
        /// Note: changes apply to FUTURE earnings only.
        /// Existing earnings keep their original commission rate (financial snapshot).
        /// </summary>
        [HttpPut("settings")]
        public async Task<IActionResult> UpdatePayoutSettings([FromBody] UpdatePayoutSettingsDto dto)
        {
            var result = await _settingsService.UpdateSettingsAsync(dto, AdminId);
            return Ok(ApiResponse<PayoutSettingsDto>.SuccessResult(result, "Payout settings updated."));
        }
    }
}
