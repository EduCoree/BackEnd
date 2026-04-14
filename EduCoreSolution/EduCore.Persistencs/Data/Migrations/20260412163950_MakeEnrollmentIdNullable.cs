using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduCore.Persistencs.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeEnrollmentIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_payments_EnrollmentId",
                table: "payments");

            migrationBuilder.AlterColumn<int>(
                name: "EnrollmentId",
                table: "payments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_payments_EnrollmentId",
                table: "payments",
                column: "EnrollmentId",
                unique: true,
                filter: "[EnrollmentId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_payments_EnrollmentId",
                table: "payments");

            migrationBuilder.AlterColumn<int>(
                name: "EnrollmentId",
                table: "payments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_payments_EnrollmentId",
                table: "payments",
                column: "EnrollmentId",
                unique: true);
        }
    }
}
