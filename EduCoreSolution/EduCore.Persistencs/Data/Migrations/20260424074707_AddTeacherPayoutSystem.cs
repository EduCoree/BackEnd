using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduCore.Persistencs.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherPayoutSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.CreateTable(
                name: "payout_settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherCommissionRate = table.Column<decimal>(type: "DECIMAL(5,4)", nullable: false),
                    Tier1Threshold = table.Column<int>(type: "int", nullable: false),
                    Tier1Bonus = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    Tier2Threshold = table.Column<int>(type: "int", nullable: false),
                    Tier2Bonus = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    Tier3Threshold = table.Column<int>(type: "int", nullable: false),
                    Tier3Bonus = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "EGP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payout_settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "teacher_invoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TeacherId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidEnrollmentsCount = table.Column<int>(type: "int", nullable: false),
                    EarningsTotal = table.Column<decimal>(type: "DECIMAL(12,2)", nullable: false),
                    TierBonus = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false, defaultValue: 0m),
                    TotalAmount = table.Column<decimal>(type: "DECIMAL(12,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "EGP"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Draft"),
                    PayoutMethod = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PayoutReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teacher_invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_teacher_invoices_AspNetUsers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "teacher_earnings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    EnrollmentId = table.Column<int>(type: "int", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    CommissionRate = table.Column<decimal>(type: "DECIMAL(5,4)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    PlatformFee = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "EGP"),
                    EarnedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    InvoiceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teacher_earnings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_teacher_earnings_AspNetUsers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_teacher_earnings_courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_teacher_earnings_enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_teacher_earnings_payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_teacher_earnings_teacher_invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "teacher_invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "payout_settings",
                columns: new[] { "Id", "Currency", "TeacherCommissionRate", "Tier1Bonus", "Tier1Threshold", "Tier2Bonus", "Tier2Threshold", "Tier3Bonus", "Tier3Threshold", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, "EGP", 0.80m, 500m, 10, 1500m, 30, 3000m, 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.CreateIndex(
                name: "IX_teacher_earnings_CourseId",
                table: "teacher_earnings",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_teacher_earnings_EnrollmentId",
                table: "teacher_earnings",
                column: "EnrollmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_teacher_earnings_InvoiceId",
                table: "teacher_earnings",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_teacher_earnings_PaymentId",
                table: "teacher_earnings",
                column: "PaymentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_teacher_earnings_TeacherId_EarnedAt",
                table: "teacher_earnings",
                columns: new[] { "TeacherId", "EarnedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_teacher_earnings_TeacherId_Status",
                table: "teacher_earnings",
                columns: new[] { "TeacherId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_teacher_invoices_InvoiceNumber",
                table: "teacher_invoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_teacher_invoices_Status",
                table: "teacher_invoices",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_teacher_invoices_TeacherId_PeriodStart_PeriodEnd",
                table: "teacher_invoices",
                columns: new[] { "TeacherId", "PeriodStart", "PeriodEnd" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payout_settings");

            migrationBuilder.DropTable(
                name: "teacher_earnings");

            migrationBuilder.DropTable(
                name: "teacher_invoices");

           
        }
    }
}
