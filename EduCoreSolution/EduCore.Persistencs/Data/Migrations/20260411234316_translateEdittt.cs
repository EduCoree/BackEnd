using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduCore.Persistencs.Data.Migrations
{
    /// <inheritdoc />
    public partial class translateEdittt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "translations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    Field = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Lang = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Value = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_translations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_translations_EntityType_EntityId_Field_Lang",
                table: "translations",
                columns: new[] { "EntityType", "EntityId", "Field", "Lang" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "translations");
        }
    }
}
