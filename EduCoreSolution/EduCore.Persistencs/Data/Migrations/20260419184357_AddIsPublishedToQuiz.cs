using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduCore.Persistencs.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPublishedToQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "quizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "quizzes");
        }
    }
}
