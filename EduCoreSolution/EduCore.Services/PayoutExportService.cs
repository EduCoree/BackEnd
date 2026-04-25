using EduCore.Domain.Contracts;
using EduCore.Services.Exports;
using EduCore.Services_Abstraction;
using EduCore.Shared.Common;
using EduCore.Shared.Exceptions;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class PayoutExportService : IPayoutExportService
    {
        private const string PdfContentType = "application/pdf";
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        private readonly IUnitOfWork _uow;

        public PayoutExportService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ExportResult> ExportInvoiceToPdfAsync(int invoiceId)
        {
            // Load invoice with all needed details (teacher + earnings + courses)
            var invoice = await _uow.TeacherInvoiceRepository.GetByIdWithDetailsAsync(invoiceId);

            if (invoice is null)
                throw new NotFoundException($"Invoice #{invoiceId} not found.");

            var teacherName = invoice.Teacher?.Name ?? "Unknown Teacher";
            var teacherEmail = invoice.Teacher?.Email;

            // Build the PDF document
            var pdfDocument = new InvoicePdfDocument(invoice, teacherName, teacherEmail);
            var pdfBytes = pdfDocument.GeneratePdf();

            // Filename: "INV-2026-04-001.pdf" (sanitized — invoice numbers don't have illegal chars)
            var fileName = $"{invoice.InvoiceNumber}.pdf";

            return new ExportResult
            {
                Content = pdfBytes,
                FileName = fileName,
                ContentType = PdfContentType
            };
        }

        public async Task<ExportResult> ExportFinancialReportToExcelAsync(DateTime from, DateTime to)
        {
            if (to < from)
                throw new BadRequestException("'to' date must be after 'from' date.");

            // Currency from settings (used for cell formatting)
            var settings = await _uow.PayoutSettingsRepository.GetSettingsAsync();

            // Pull all invoices in the period (across all teachers)
            // PageSize = int.MaxValue effectively pulls all rows for the report.
            // For very large datasets (10,000+ invoices) we'd need a different
            // streaming approach — but at typical scale this is fine.
            var allInvoices = await _uow.TeacherInvoiceRepository.GetAllInvoicesPagedAsync(
                status: null,
                teacherId: null,
                from: from,
                to: to,
                pagination: new PaginationParams { PageNumber = 1, PageSize = int.MaxValue });

            // Pull all earnings in the period (with teacher + course + invoice loaded)
            var allEarnings = (await _uow.TeacherEarningRepository
                .GetEarningsForReportAsync(from, to))
                .ToList();

            // Re-fetch each earning's invoice number (the report repo doesn't include it)
            // We avoid an extra DB call by mapping from the invoices we already loaded.
            var invoiceMap = allInvoices.Items.ToDictionary(i => i.Id, i => i);
            foreach (var earning in allEarnings)
            {
                if (earning.InvoiceId.HasValue && invoiceMap.TryGetValue(earning.InvoiceId.Value, out var inv))
                {
                    earning.Invoice = inv;
                }
            }

            // Generate the workbook
            var generator = new FinancialReportExcelGenerator(
                from, to,
                allInvoices.Items.ToList(),
                allEarnings,
                settings.Currency);

            var bytes = generator.Generate();

            // Filename: "EduCore-FinancialReport-2026-01-01_2026-04-30.xlsx"
            var fileName = $"EduCore-FinancialReport-{from:yyyy-MM-dd}_{to:yyyy-MM-dd}.xlsx";

            return new ExportResult
            {
                Content = bytes,
                FileName = fileName,
                ContentType = ExcelContentType
            };
        }
    }
}
