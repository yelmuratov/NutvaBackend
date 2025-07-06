using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutvaCms.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTranslationSlugs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "Products",
                newName: "Uz_Slug");

            migrationBuilder.AddColumn<string>(
                name: "En_Slug",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ru_Slug",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "En_Slug",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Ru_Slug",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Uz_Slug",
                table: "Products",
                newName: "Slug");
        }
    }
}
