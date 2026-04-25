using ClosedXML.Excel;
using EduCore.Domain.Entities.PayoutModel;
using EduCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services.Exports
{
    /// <summary>
    /// Builds a 3-sheet Excel workbook summarizing payout activity in a period.
    ///   Sheet 1: Summary (KPIs)
    ///   Sheet 2: Invoices  (one row per invoice)
    ///   Sheet 3: Earnings  (one row per individual earning)
    /// </summary>
    public class FinancialReportExcelGenerator
    {
        // Colors used across the workbook
        private static readonly XLColor HeaderBg = XLColor.FromHtml("#1976D2");
        private static readonly XLColor HeaderFg = XLColor.White;
        private static readonly XLColor SubtleBg = XLColor.FromHtml("#E3F2FD");
        private static readonly XLColor BorderColor = XLColor.FromHtml("#E0E0E0");

        private readonly DateTime _from;
        private readonly DateTime _to;
        private readonly IReadOnlyList<TeacherInvoice> _invoices;
        private readonly IReadOnlyList<TeacherEarning> _earnings;
        private readonly string _currency;

        public FinancialReportExcelGenerator(
            DateTime from,
            DateTime to,
            IReadOnlyList<TeacherInvoice> invoices,
            IReadOnlyList<TeacherEarning> earnings,
            string currency)
        {
            _from = from;
            _to = to;
            _invoices = invoices;
            _earnings = earnings;
            _currency = currency;
        }

        public byte[] Generate()
        {
            using var workbook = new XLWorkbook();

            BuildSummarySheet(workbook);
            BuildInvoicesSheet(workbook);
            BuildEarningsSheet(workbook);

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        // ═══════════════════════════════════════════════════════════════
        //  Sheet 1: Summary
        // ═══════════════════════════════════════════════════════════════
        private void BuildSummarySheet(XLWorkbook workbook)
        {
            var sheet = workbook.Worksheets.Add("Summary");

            // Title
            sheet.Cell("A1").Value = "EduCore — Payout Financial Report";
            sheet.Cell("A1").Style.Font.SetFontSize(16).Font.SetBold();
            sheet.Range("A1:D1").Merge();

            // Period
            sheet.Cell("A2").Value = $"Period: {_from:dd MMM yyyy} – {_to:dd MMM yyyy}";
            sheet.Cell("A2").Style.Font.SetFontSize(11).Font.SetItalic();
            sheet.Range("A2:D2").Merge();

            sheet.Cell("A3").Value = $"Generated: {DateTime.UtcNow:dd MMM yyyy HH:mm} UTC";
            sheet.Cell("A3").Style.Font.SetFontSize(9).Font.SetFontColor(XLColor.Gray);
            sheet.Range("A3:D3").Merge();

            // KPI Section header
            sheet.Cell("A5").Value = "KEY METRICS";
            sheet.Cell("A5").Style.Font.SetBold().Font.SetFontColor(HeaderBg);
            sheet.Range("A5:D5").Style.Border.SetBottomBorder(XLBorderStyleValues.Medium)
                .Border.SetBottomBorderColor(HeaderBg);

            // Compute the metrics
            var totalGross = _earnings.Sum(e => e.GrossAmount);
            var totalTeacherEarnings = _earnings.Where(e => e.Status != EarningStatus.Cancelled).Sum(e => e.NetAmount);
            var totalPlatformRevenue = _earnings.Where(e => e.Status != EarningStatus.Cancelled).Sum(e => e.PlatformFee);
            var totalEnrollments = _earnings.Count(e => e.Status != EarningStatus.Cancelled);
            var distinctTeachers = _earnings.Select(e => e.TeacherId).Distinct().Count();
            var totalInvoices = _invoices.Count;
            var paidInvoices = _invoices.Count(i => i.Status == InvoiceStatus.Paid);
            var pendingInvoices = _invoices.Count(i => i.Status == InvoiceStatus.Issued || i.Status == InvoiceStatus.Draft);
            var totalPaidOut = _invoices.Where(i => i.Status == InvoiceStatus.Paid).Sum(i => i.TotalAmount);
            var totalPendingPayouts = _invoices
                .Where(i => i.Status == InvoiceStatus.Issued || i.Status == InvoiceStatus.Draft)
                .Sum(i => i.TotalAmount);
            var totalTierBonuses = _invoices.Where(i => i.Status != InvoiceStatus.Cancelled).Sum(i => i.TierBonus);

            int row = 7;
            AddKpiRow(sheet, ref row, "Total Gross Revenue", totalGross, true);
            AddKpiRow(sheet, ref row, "Teacher Earnings (80%)", totalTeacherEarnings, false);
            AddKpiRow(sheet, ref row, "Platform Revenue (20%)", totalPlatformRevenue, true);
            AddKpiRow(sheet, ref row, "Tier Bonuses Paid", totalTierBonuses, false);

            row++; // gap
            AddKpiRow(sheet, ref row, "Total Paid Enrollments", totalEnrollments, false, isMoney: false);
            AddKpiRow(sheet, ref row, "Active Teachers", distinctTeachers, true, isMoney: false);

            row++; // gap
            AddKpiRow(sheet, ref row, "Total Invoices Generated", totalInvoices, false, isMoney: false);
            AddKpiRow(sheet, ref row, "  → Paid", paidInvoices, true, isMoney: false);
            AddKpiRow(sheet, ref row, "  → Pending", pendingInvoices, false, isMoney: false);

            row++; // gap
            AddKpiRow(sheet, ref row, "Total Paid Out to Teachers", totalPaidOut, true);
            AddKpiRow(sheet, ref row, "Pending Payouts (Owed to Teachers)", totalPendingPayouts, false);

            // Column widths
            sheet.Column(1).Width = 38;
            sheet.Column(2).Width = 22;

            // Freeze top rows
            sheet.SheetView.FreezeRows(5);
        }

        private void AddKpiRow(IXLWorksheet sheet, ref int row, string label, decimal value, bool zebra, bool isMoney = true)
        {
            sheet.Cell(row, 1).Value = label;
            sheet.Cell(row, 2).Value = value;

            if (isMoney)
            {
                sheet.Cell(row, 2).Style.NumberFormat.Format = $"#,##0.00 \"{_currency}\"";
            }
            else
            {
                sheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0";
            }

            sheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            sheet.Cell(row, 1).Style.Font.SetBold(label.StartsWith("Total ") || label.StartsWith("Active "));

            if (zebra)
            {
                sheet.Range(row, 1, row, 2).Style.Fill.SetBackgroundColor(SubtleBg);
            }

            row++;
        }

        // ═══════════════════════════════════════════════════════════════
        //  Sheet 2: Invoices
        // ═══════════════════════════════════════════════════════════════
        private void BuildInvoicesSheet(XLWorkbook workbook)
        {
            var sheet = workbook.Worksheets.Add("Invoices");

            // Headers
            string[] headers =
            {
                "Invoice #", "Teacher", "Period Start", "Period End",
                "Paid Enrollments", "Earnings Total", "Tier Bonus",
                "Total Amount", "Status", "Issued At", "Paid At",
                "Payout Method", "Reference"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = sheet.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Fill.SetBackgroundColor(HeaderBg);
                cell.Style.Font.SetFontColor(HeaderFg).Font.SetBold();
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            int row = 2;
            foreach (var inv in _invoices.OrderByDescending(i => i.CreatedAt))
            {
                sheet.Cell(row, 1).Value = inv.InvoiceNumber;
                sheet.Cell(row, 2).Value = inv.Teacher?.Name ?? inv.TeacherId;
                sheet.Cell(row, 3).Value = inv.PeriodStart;
                sheet.Cell(row, 4).Value = inv.PeriodEnd;
                sheet.Cell(row, 5).Value = inv.PaidEnrollmentsCount;
                sheet.Cell(row, 6).Value = inv.EarningsTotal;
                sheet.Cell(row, 7).Value = inv.TierBonus;
                sheet.Cell(row, 8).Value = inv.TotalAmount;
                sheet.Cell(row, 9).Value = inv.Status.ToString();
                sheet.Cell(row, 10).Value = inv.IssuedAt;
                sheet.Cell(row, 11).Value = inv.PaidAt;
                sheet.Cell(row, 12).Value = inv.PayoutMethod?.ToString();
                sheet.Cell(row, 13).Value = inv.PayoutReference;

                // Format dates
                sheet.Cell(row, 3).Style.DateFormat.Format = "yyyy-mm-dd";
                sheet.Cell(row, 4).Style.DateFormat.Format = "yyyy-mm-dd";
                sheet.Cell(row, 10).Style.DateFormat.Format = "yyyy-mm-dd";
                sheet.Cell(row, 11).Style.DateFormat.Format = "yyyy-mm-dd";

                // Format money columns
                sheet.Cell(row, 6).Style.NumberFormat.Format = $"#,##0.00 \"{_currency}\"";
                sheet.Cell(row, 7).Style.NumberFormat.Format = $"#,##0.00 \"{_currency}\"";
                sheet.Cell(row, 8).Style.NumberFormat.Format = $"#,##0.00 \"{_currency}\"";
                sheet.Cell(row, 8).Style.Font.SetBold();

                // Color the status cell
                var statusCell = sheet.Cell(row, 9);
                statusCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                statusCell.Style.Fill.SetBackgroundColor(inv.Status switch
                {
                    InvoiceStatus.Paid => XLColor.FromHtml("#C8E6C9"),
                    InvoiceStatus.Cancelled => XLColor.FromHtml("#FFCDD2"),
                    InvoiceStatus.Issued => XLColor.FromHtml("#FFE0B2"),
                    _ => XLColor.LightGray
                });

                row++;
            }

            // Convert to a table for sorting/filtering
            if (_invoices.Count > 0)
            {
                var range = sheet.Range(1, 1, row - 1, headers.Length);
                var table = range.CreateTable("InvoicesTable");
                table.Theme = XLTableTheme.TableStyleLight2;
            }

            sheet.Columns().AdjustToContents();
            sheet.SheetView.FreezeRows(1);
        }

        // ═══════════════════════════════════════════════════════════════
        //  Sheet 3: Earnings
        // ═══════════════════════════════════════════════════════════════
        private void BuildEarningsSheet(XLWorkbook workbook)
        {
            var sheet = workbook.Worksheets.Add("Earnings");

            string[] headers =
            {
                "Earned At", "Teacher", "Course", "Gross Amount",
                "Commission Rate", "Net Amount", "Platform Fee",
                "Status", "Invoice #"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = sheet.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Fill.SetBackgroundColor(HeaderBg);
                cell.Style.Font.SetFontColor(HeaderFg).Font.SetBold();
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            int row = 2;
            foreach (var e in _earnings.OrderByDescending(x => x.EarnedAt))
            {
                sheet.Cell(row, 1).Value = e.EarnedAt;
                sheet.Cell(row, 2).Value = e.Teacher?.Name ?? e.TeacherId;
                sheet.Cell(row, 3).Value = e.Course?.Title ?? $"Course #{e.CourseId}";
                sheet.Cell(row, 4).Value = e.GrossAmount;
                sheet.Cell(row, 5).Value = e.CommissionRate;
                sheet.Cell(row, 6).Value = e.NetAmount;
                sheet.Cell(row, 7).Value = e.PlatformFee;
                sheet.Cell(row, 8).Value = e.Status.ToString();
                sheet.Cell(row, 9).Value = e.Invoice?.InvoiceNumber;

                sheet.Cell(row, 1).Style.DateFormat.Format = "yyyy-mm-dd";
                sheet.Cell(row, 4).Style.NumberFormat.Format = $"#,##0.00 \"{_currency}\"";
                sheet.Cell(row, 5).Style.NumberFormat.Format = "0.00%";
                sheet.Cell(row, 6).Style.NumberFormat.Format = $"#,##0.00 \"{_currency}\"";
                sheet.Cell(row, 7).Style.NumberFormat.Format = $"#,##0.00 \"{_currency}\"";

                row++;
            }

            // Totals row at the bottom
            if (_earnings.Count > 0)
            {
                int totalsRow = row;
                sheet.Cell(totalsRow, 1).Value = "TOTAL";
                sheet.Cell(totalsRow, 1).Style.Font.SetBold();

                sheet.Cell(totalsRow, 4).FormulaA1 = $"=SUM(D2:D{row - 1})";
                sheet.Cell(totalsRow, 6).FormulaA1 = $"=SUM(F2:F{row - 1})";
                sheet.Cell(totalsRow, 7).FormulaA1 = $"=SUM(G2:G{row - 1})";

                sheet.Range(totalsRow, 1, totalsRow, headers.Length)
                    .Style.Fill.SetBackgroundColor(SubtleBg)
                    .Font.SetBold();

                sheet.Cell(totalsRow, 4).Style.NumberFormat.Format = $"#,##0.00 \"{_currency}\"";
                sheet.Cell(totalsRow, 6).Style.NumberFormat.Format = $"#,##0.00 \"{_currency}\"";
                sheet.Cell(totalsRow, 7).Style.NumberFormat.Format = $"#,##0.00 \"{_currency}\"";
            }

            sheet.Columns().AdjustToContents();
            sheet.SheetView.FreezeRows(1);
        }
    }
}
