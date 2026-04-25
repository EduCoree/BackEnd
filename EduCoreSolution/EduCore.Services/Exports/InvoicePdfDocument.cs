using EduCore.Domain.Entities.PayoutModel;
using EduCore.Shared.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// Resolve the IContainer ambiguity at the file level.
// QuestPDF and System.ComponentModel both define IContainer;
// here we ALWAYS mean QuestPDF's.
using IContainer = QuestPDF.Infrastructure.IContainer;

namespace EduCore.Services.Exports
{
    /// <summary>
    /// QuestPDF document for a single teacher invoice.
    /// Implements IDocument so QuestPDF can render it via .GeneratePdf().
    /// </summary>
    public class InvoicePdfDocument : IDocument
    {
        private const string CompanyName = "EduCore";

        // Color palette as plain hex strings (QuestPDF accepts strings for colors)
        private const string PrimaryColor = "#1976D2";   // Blue
        private const string AccentColor = "#E3F2FD";   // Light blue
        private const string TextColor = "#212121";   // Dark gray
        private const string MutedColor = "#757575";   // Muted gray
        private const string BorderColor = "#E0E0E0";   // Light gray

        private readonly TeacherInvoice _invoice;
        private readonly string _teacherName;
        private readonly string? _teacherEmail;

        public InvoicePdfDocument(TeacherInvoice invoice, string teacherName, string? teacherEmail)
        {
            _invoice = invoice;
            _teacherName = teacherName;
            _teacherEmail = teacherEmail;
        }

        public DocumentMetadata GetMetadata() => new()
        {
            Title = $"Invoice {_invoice.InvoiceNumber}",
            Author = CompanyName,
            Subject = $"Teacher Payout Invoice — {_teacherName}",
            CreationDate = DateTime.UtcNow
        };

        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(t => t.FontSize(10).FontColor(TextColor));

                // Lambdas instead of method-group references — works on all QuestPDF versions
                page.Header().Element(c => ComposeHeader(c));
                page.Content().Element(c => ComposeContent(c));
                page.Footer().Element(c => ComposeFooter(c));
            });
        }

        // ─── Header (top of every page) ──────────────────────────────
        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                // Left: company name + tagline
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text(CompanyName)
                        .FontSize(24).Bold().FontColor(PrimaryColor);

                    col.Item().Text("Teacher Payout Invoice")
                        .FontSize(11).FontColor(MutedColor);
                });

                // Right: invoice number & dates
                row.RelativeItem().Column(col =>
                {
                    col.Item().AlignRight().Text("INVOICE")
                        .FontSize(20).Bold().FontColor(PrimaryColor);

                    col.Item().AlignRight().Text(_invoice.InvoiceNumber)
                        .FontSize(12).SemiBold();

                    var issuedText = _invoice.IssuedAt?.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) ?? "—";
                    col.Item().AlignRight().Text($"Issued: {issuedText}")
                        .FontSize(9).FontColor(MutedColor);
                });
            });
        }

        // ─── Content (main body) ─────────────────────────────────────
        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(col =>
            {
                col.Spacing(15);

                col.Item().Element(c => ComposeBillToPeriod(c));
                col.Item().Element(c => ComposeEarningsTable(c));
                col.Item().Element(c => ComposeTotals(c));
                col.Item().Element(c => ComposeStatusBanner(c));
            });
        }

        private void ComposeBillToPeriod(IContainer container)
        {
            container.Row(row =>
            {
                // Bill To
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("BILL TO").FontSize(9).Bold().FontColor(MutedColor);
                    col.Item().PaddingTop(4).Text(_teacherName).FontSize(12).SemiBold();

                    if (!string.IsNullOrEmpty(_teacherEmail))
                        col.Item().Text(_teacherEmail).FontSize(9).FontColor(MutedColor);
                });

                // Period
                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().AlignRight().Text("PERIOD").FontSize(9).Bold().FontColor(MutedColor);
                    col.Item().AlignRight().PaddingTop(4)
                        .Text(_invoice.PeriodStart.ToString("MMMM yyyy", CultureInfo.InvariantCulture))
                        .FontSize(12).SemiBold();

                    col.Item().AlignRight()
                        .Text($"{_invoice.PeriodStart:dd MMM} – {_invoice.PeriodEnd:dd MMM yyyy}")
                        .FontSize(9).FontColor(MutedColor);
                });
            });
        }

        private void ComposeEarningsTable(IContainer container)
        {
            container.Table(table =>
            {
                // Columns: # | Course | Date | Gross | Rate | Net
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(25);     // #
                    c.RelativeColumn(3);      // Course
                    c.RelativeColumn(1.2f);   // Date
                    c.RelativeColumn(1.2f);   // Gross
                    c.ConstantColumn(50);     // Rate
                    c.RelativeColumn(1.2f);   // Net
                });

                // Header
                table.Header(h =>
                {
                    h.Cell().Background(PrimaryColor).Padding(6).Text("#").FontColor(Colors.White).FontSize(9).Bold();
                    h.Cell().Background(PrimaryColor).Padding(6).Text("Course").FontColor(Colors.White).FontSize(9).Bold();
                    h.Cell().Background(PrimaryColor).Padding(6).Text("Date").FontColor(Colors.White).FontSize(9).Bold();
                    h.Cell().Background(PrimaryColor).Padding(6).AlignRight().Text("Gross").FontColor(Colors.White).FontSize(9).Bold();
                    h.Cell().Background(PrimaryColor).Padding(6).AlignRight().Text("Rate").FontColor(Colors.White).FontSize(9).Bold();
                    h.Cell().Background(PrimaryColor).Padding(6).AlignRight().Text("Net").FontColor(Colors.White).FontSize(9).Bold();
                });

                // Rows
                int rowNum = 1;
                var orderedEarnings = _invoice.Earnings.OrderBy(e => e.EarnedAt).ToList();

                foreach (var earning in orderedEarnings)
                {
                    var background = rowNum % 2 == 0 ? AccentColor : "#FFFFFF";
                    var courseTitle = earning.Course?.Title ?? $"Course #{earning.CourseId}";

                    table.Cell().Background(background).Padding(6).Text(rowNum.ToString()).FontSize(9);
                    table.Cell().Background(background).Padding(6).Text(courseTitle).FontSize(9);
                    table.Cell().Background(background).Padding(6)
                         .Text(earning.EarnedAt.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)).FontSize(9);
                    table.Cell().Background(background).Padding(6).AlignRight()
                         .Text(FormatMoney(earning.GrossAmount, earning.Currency)).FontSize(9);
                    table.Cell().Background(background).Padding(6).AlignRight()
                         .Text($"{earning.CommissionRate:P0}").FontSize(9);
                    table.Cell().Background(background).Padding(6).AlignRight()
                         .Text(FormatMoney(earning.NetAmount, earning.Currency)).FontSize(9).SemiBold();

                    rowNum++;
                }

                // Empty state — invoice with no earnings (shouldn't happen, but fail-safe)
                if (orderedEarnings.Count == 0)
                {
                    table.Cell().ColumnSpan(6).Padding(12).AlignCenter()
                         .Text("No earnings in this period.").FontSize(10).FontColor(MutedColor).Italic();
                }
            });
        }

        private void ComposeTotals(IContainer container)
        {
            container.AlignRight().Width(280).Column(col =>
            {
                col.Spacing(4);

                // Earnings subtotal
                col.Item().Row(row =>
                {
                    row.RelativeItem().Text("Earnings Subtotal").FontSize(10).FontColor(MutedColor);
                    row.ConstantItem(110).AlignRight()
                        .Text(FormatMoney(_invoice.EarningsTotal, _invoice.Currency)).FontSize(10);
                });

                // Tier bonus (only show if > 0)
                if (_invoice.TierBonus > 0)
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem()
                            .Text($"Tier Bonus ({_invoice.PaidEnrollmentsCount} enrollments)")
                            .FontSize(10).FontColor(MutedColor);
                        row.ConstantItem(110).AlignRight()
                            .Text(FormatMoney(_invoice.TierBonus, _invoice.Currency)).FontSize(10);
                    });
                }

                // Divider
                col.Item().PaddingTop(4).BorderTop(1).BorderColor(BorderColor);

                // Total
                col.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem().Text("TOTAL").FontSize(13).Bold();
                    row.ConstantItem(110).AlignRight()
                        .Text(FormatMoney(_invoice.TotalAmount, _invoice.Currency))
                        .FontSize(13).Bold().FontColor(PrimaryColor);
                });
            });
        }

        private void ComposeStatusBanner(IContainer container)
        {
            // Pick colors as STRINGS (uniform type) to avoid CS0172 ambiguity.
            string bgColor;
            string textColor;
            string statusText;

            switch (_invoice.Status)
            {
                case InvoiceStatus.Paid:
                    bgColor = "#E8F5E9"; textColor = "#2E7D32"; statusText = "PAID"; break;
                case InvoiceStatus.Cancelled:
                    bgColor = "#FFEBEE"; textColor = "#C62828"; statusText = "CANCELLED"; break;
                case InvoiceStatus.Issued:
                    bgColor = "#FFF3E0"; textColor = "#E65100"; statusText = "AWAITING PAYMENT"; break;
                default:
                    bgColor = AccentColor; textColor = PrimaryColor;
                    statusText = _invoice.Status.ToString().ToUpper();
                    break;
            }

            container.PaddingTop(20).Background(bgColor).Padding(12).Row(row =>
            {
                // Status
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("STATUS").FontSize(8).Bold().FontColor(MutedColor);
                    col.Item().PaddingTop(2).Text(statusText).FontSize(12).Bold().FontColor(textColor);
                });

                // Right-side details (paid info or enrollments count)
                if (_invoice.Status == InvoiceStatus.Paid)
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().AlignRight().Text("PAID ON").FontSize(8).Bold().FontColor(MutedColor);
                        var paidText = _invoice.PaidAt?.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) ?? "—";
                        col.Item().AlignRight().PaddingTop(2)
                            .Text(paidText).FontSize(10).SemiBold();

                        if (!string.IsNullOrEmpty(_invoice.PayoutReference))
                            col.Item().AlignRight()
                                .Text($"Ref: {_invoice.PayoutReference}").FontSize(8).FontColor(MutedColor);
                    });
                }
                else
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().AlignRight().Text("PAID ENROLLMENTS").FontSize(8).Bold().FontColor(MutedColor);
                        col.Item().AlignRight().PaddingTop(2)
                            .Text(_invoice.PaidEnrollmentsCount.ToString())
                            .FontSize(12).SemiBold();
                    });
                }
            });
        }

        // ─── Footer (bottom of every page) ───────────────────────────
        private void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().PaddingTop(10).BorderTop(1).BorderColor(BorderColor);

                col.Item().PaddingTop(8).Row(row =>
                {
                    row.RelativeItem()
                        .Text(t =>
                        {
                            t.Span($"{CompanyName}  |  ").FontSize(8).FontColor(MutedColor);
                            t.Span("Teacher Payout System").FontSize(8).FontColor(MutedColor);
                        });

                    row.RelativeItem().AlignRight().Text(t =>
                    {
                        t.Span("Page ").FontSize(8).FontColor(MutedColor);
                        t.CurrentPageNumber().FontSize(8).FontColor(MutedColor);
                        t.Span(" of ").FontSize(8).FontColor(MutedColor);
                        t.TotalPages().FontSize(8).FontColor(MutedColor);
                    });
                });
            });
        }

        // ─── Helpers ─────────────────────────────────────────────────
        private static string FormatMoney(decimal amount, string currency)
        {
            // Example: "1,250.00 EGP"
            return $"{amount.ToString("N2", CultureInfo.InvariantCulture)} {currency}";
        }
    }
}
