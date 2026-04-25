using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services_Abstraction
{
    /// <summary>
    /// Service for exporting payout data to PDF and Excel formats.
    /// Returns raw byte arrays that the controller streams as file downloads.
    /// </summary>
    public interface IPayoutExportService
    {
        /// <summary>
        /// Generates a PDF document for a single invoice.
        /// Used by both teacher (their own invoice) and admin (any invoice).
        /// Returns the file bytes + suggested filename.
        /// </summary>
        Task<ExportResult> ExportInvoiceToPdfAsync(int invoiceId);

        /// <summary>
        /// Generates a multi-sheet Excel workbook with a financial report
        /// for the given period (Summary + Invoices + Earnings sheets).
        /// </summary>
        /// <param name="from">Period start (inclusive)</param>
        /// <param name="to">Period end (inclusive)</param>
        Task<ExportResult> ExportFinancialReportToExcelAsync(DateTime from, DateTime to);
    }

    /// <summary>
    /// Carries the file content and metadata back to the controller.
    /// </summary>
    public class ExportResult
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
    }
}
