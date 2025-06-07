using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutvaCms.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBannerTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Banners",
                newName: "Uz_Title");

            migrationBuilder.RenameColumn(
                name: "Subtitle",
                table: "Banners",
                newName: "Uz_Subtitle");

            migrationBuilder.RenameColumn(
                name: "MetaTitle",
                table: "Banners",
                newName: "Uz_MetaTitle");

            migrationBuilder.RenameColumn(
                name: "MetaKeywords",
                table: "Banners",
                newName: "Uz_MetaKeywords");

            migrationBuilder.RenameColumn(
                name: "MetaDescription",
                table: "Banners",
                newName: "Uz_MetaDescription");

            migrationBuilder.AddColumn<string>(
                name: "En_MetaDescription",
                table: "Banners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "En_MetaKeywords",
                table: "Banners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "En_MetaTitle",
                table: "Banners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "En_Subtitle",
                table: "Banners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "En_Title",
                table: "Banners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ru_MetaDescription",
                table: "Banners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ru_MetaKeywords",
                table: "Banners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ru_MetaTitle",
                table: "Banners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ru_Subtitle",
                table: "Banners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ru_Title",
                table: "Banners",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "En_MetaDescription",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "En_MetaKeywords",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "En_MetaTitle",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "En_Subtitle",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "En_Title",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "Ru_MetaDescription",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "Ru_MetaKeywords",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "Ru_MetaTitle",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "Ru_Subtitle",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "Ru_Title",
                table: "Banners");

            migrationBuilder.RenameColumn(
                name: "Uz_Title",
                table: "Banners",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Uz_Subtitle",
                table: "Banners",
                newName: "Subtitle");

            migrationBuilder.RenameColumn(
                name: "Uz_MetaTitle",
                table: "Banners",
                newName: "MetaTitle");

            migrationBuilder.RenameColumn(
                name: "Uz_MetaKeywords",
                table: "Banners",
                newName: "MetaKeywords");

            migrationBuilder.RenameColumn(
                name: "Uz_MetaDescription",
                table: "Banners",
                newName: "MetaDescription");
        }
    }
}
