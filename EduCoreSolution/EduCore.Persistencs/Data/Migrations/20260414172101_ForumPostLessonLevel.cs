using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduCore.Persistencs.Data.Migrations
{
    /// <inheritdoc />
    public partial class ForumPostLessonLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_forum_posts_courses_CourseId",
                table: "forum_posts");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "forum_posts",
                newName: "LessonId");

            migrationBuilder.RenameIndex(
                name: "IX_forum_posts_CourseId",
                table: "forum_posts",
                newName: "IX_forum_posts_LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_forum_posts_lessons_LessonId",
                table: "forum_posts",
                column: "LessonId",
                principalTable: "lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_forum_posts_lessons_LessonId",
                table: "forum_posts");

            migrationBuilder.RenameColumn(
                name: "LessonId",
                table: "forum_posts",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_forum_posts_LessonId",
                table: "forum_posts",
                newName: "IX_forum_posts_CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_forum_posts_courses_CourseId",
                table: "forum_posts",
                column: "CourseId",
                principalTable: "courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
