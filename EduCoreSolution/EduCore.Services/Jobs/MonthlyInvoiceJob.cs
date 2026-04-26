using EduCore.Services_Abstraction;
using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services.Jobs
{
    /// <summary>
    /// Hangfire recurring job that generates teacher invoices for the previous month.
    /// Scheduled to run on day 1 of each month at 02:00 UTC.
    /// </summary>
    public class MonthlyInvoiceJob
    {
        public const string JobId = "monthly-teacher-invoices";

        private readonly IInvoiceGenerationService _invoiceGenerationService;
        private readonly ILogger<MonthlyInvoiceJob> _logger;

        public MonthlyInvoiceJob(
            IInvoiceGenerationService invoiceGenerationService,
            ILogger<MonthlyInvoiceJob> logger)
        {
            _invoiceGenerationService = invoiceGenerationService;
            _logger = logger;
        }

        /// <summary>
        /// The actual work. Kept simple — just delegates to the generation service.
        /// Hangfire will automatically retry on exception (up to 10 times by default).
        /// 
        /// DisableConcurrentExecution prevents two instances from running at the same time
        /// (e.g., if a previous run is still going when the next month rolls over).
        /// </summary>
        [DisableConcurrentExecution(timeoutInSeconds: 30 * 60)] // 30 min max wait
        [AutomaticRetry(Attempts = 3)]
        public async Task RunAsync()
        {
            _logger.LogInformation("MonthlyInvoiceJob started at {UtcNow}", DateTime.UtcNow);

            var result = await _invoiceGenerationService.GenerateForPreviousMonthAsync();

            _logger.LogInformation(
                "MonthlyInvoiceJob completed. Period: {Year}-{Month:D2}. " +
                "Invoices created: {Created}, Skipped: {Skipped}, Failed: {Failed}, Total: {Total}",
                result.Year, result.Month,
                result.InvoicesCreated, result.TeachersSkipped, result.TeachersFailed,
                result.TotalAmountGenerated);

            // If any teacher failed, let Hangfire know so it retries.
            // (But successful teachers are already committed — no data loss.)
            if (result.TeachersFailed > 0)
            {
                throw new InvalidOperationException(
                    $"Invoice generation had {result.TeachersFailed} failures. " +
                    $"Failed teachers: {string.Join(", ", result.FailedTeacherIds)}. " +
                    $"Check logs for details. Hangfire will retry.");
            }
        }
    }
}
