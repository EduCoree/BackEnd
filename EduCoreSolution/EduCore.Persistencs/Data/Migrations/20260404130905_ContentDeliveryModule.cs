using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduCore.Persistencs.Data.Migrations
{
    /// <inheritdoc />
    public partial class ContentDeliveryModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_live_sessions_lessons_LessonId",
                table: "live_sessions");

            migrationBuilder.DropIndex(
                name: "IX_live_sessions_LessonId",
                table: "live_sessions");

            migrationBuilder.AlterColumn<int>(
                name: "LessonId",
                table: "live_sessions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "live_sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "live_sessions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "live_sessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "live_sessions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "lessons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_live_sessions_CourseId",
                table: "live_sessions",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_live_sessions_LessonId",
                table: "live_sessions",
                column: "LessonId",
                unique: true,
                filter: "[LessonId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_live_sessions_courses_CourseId",
                table: "live_sessions",
                column: "CourseId",
                principalTable: "courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_live_sessions_lessons_LessonId",
                table: "live_sessions",
                column: "LessonId",
                principalTable: "lessons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_live_sessions_courses_CourseId",
                table: "live_sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_live_sessions_lessons_LessonId",
                table: "live_sessions");

            migrationBuilder.DropIndex(
                name: "IX_live_sessions_CourseId",
                table: "live_sessions");

            migrationBuilder.DropIndex(
                name: "IX_live_sessions_LessonId",
                table: "live_sessions");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "live_sessions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "live_sessions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "live_sessions");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "live_sessions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "lessons");

            migrationBuilder.AlterColumn<int>(
                name: "LessonId",
                table: "live_sessions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_live_sessions_LessonId",
                table: "live_sessions",
                column: "LessonId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_live_sessions_lessons_LessonId",
                table: "live_sessions",
                column: "LessonId",
                principalTable: "lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
