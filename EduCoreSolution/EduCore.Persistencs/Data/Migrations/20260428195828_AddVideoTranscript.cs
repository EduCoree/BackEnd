using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduCore.Persistencs.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoTranscript : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TranscribedAt",
                table: "video_lessons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Transcript",
                table: "video_lessons",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TranscribedAt",
                table: "video_lessons");

            migrationBuilder.DropColumn(
                name: "Transcript",
                table: "video_lessons");
        }
    }
}
