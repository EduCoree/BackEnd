using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.AuthModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.Exceptions;
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
    /// <summary>
    /// Export endpoints for the payout system.
    /// Kept as a separate controller (not added to AdminPayoutController or
    /// TeacherPayoutController) to keep responsibilities clean.
    /// </summary>
    [ApiController]
    [Route("api/payout/export")]
    [Authorize] // Both Teacher and Admin can hit this — per-endpoint role checks below
    public class PayoutExportController : ControllerBase
    {
        private readonly IPayoutExportService _exportService;
        private readonly IUnitOfWork _uow;

        public PayoutExportController(
            IPayoutExportService exportService,
            IUnitOfWork uow)
        {
            _exportService = exportService;
            _uow = uow;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // ─── PDF: Invoice download ─────────────────────────────────────

        /// <summary>
        /// Downloads a single invoice as a PDF.
        ///   • Teachers: only their own invoices (returns 404 if owned by someone else)
        ///   • Admins: any invoice
        /// </summary>
        [HttpGet("invoices/{invoiceId:int}/pdf")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> DownloadInvoicePdf(int invoiceId)
        {
            // Authorization: if the caller is a teacher, verify they own the invoice
            if (User.IsInRole("Teacher") && !User.IsInRole("Admin"))
            {
                var invoice = await _uow.TeacherInvoiceRepository.GetByIdAsync(invoiceId);
                if (invoice is null || invoice.TeacherId != CurrentUserId)
                {
                    // 404 (not 403) — don't leak the existence of someone else's invoice
                    throw new NotFoundException($"Invoice #{invoiceId} not found.");
                }
            }

            var result = await _exportService.ExportInvoiceToPdfAsync(invoiceId);
            return File(result.Content, result.ContentType, result.FileName);
        }

        // ─── Excel: Admin financial report ─────────────────────────────

        /// <summary>
        /// Downloads the financial report as an Excel workbook (Summary + Invoices + Earnings).
        /// Admin only.
        /// </summary>
        /// <param name="from">Start of period (inclusive). Default: 30 days ago.</param>
        /// <param name="to">End of period (inclusive). Default: today.</param>
        [HttpGet("report/financial.xlsx")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DownloadFinancialReport(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            // Sensible defaults: last 30 days
            var fromUtc = (from ?? DateTime.UtcNow.AddDays(-30)).Date;
            var toUtc = (to ?? DateTime.UtcNow).Date.AddDays(1).AddTicks(-1);

            var result = await _exportService.ExportFinancialReportToExcelAsync(fromUtc, toUtc);
            return File(result.Content, result.ContentType, result.FileName);
        }
    }
}
