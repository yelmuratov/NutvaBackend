using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutvaCms.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Products",
                newName: "Uz_Name");

            migrationBuilder.RenameColumn(
                name: "MetaTitle",
                table: "Products",
                newName: "Uz_MetaTitle");

            migrationBuilder.RenameColumn(
                name: "MetaKeywords",
                table: "Products",
                newName: "Uz_MetaKeywords");

            migrationBuilder.RenameColumn(
                name: "MetaDescription",
                table: "Products",
                newName: "Uz_MetaDescription");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Products",
                newName: "Uz_Description");

            migrationBuilder.AddColumn<string>(
                name: "En_Description",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "En_MetaDescription",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "En_MetaKeywords",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "En_MetaTitle",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "En_Name",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ru_Description",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ru_MetaDescription",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ru_MetaKeywords",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ru_MetaTitle",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ru_Name",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Products",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "En_Description",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "En_MetaDescription",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "En_MetaKeywords",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "En_MetaTitle",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "En_Name",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Ru_Description",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Ru_MetaDescription",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Ru_MetaKeywords",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Ru_MetaTitle",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Ru_Name",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Uz_Name",
                table: "Products",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Uz_MetaTitle",
                table: "Products",
                newName: "MetaTitle");

            migrationBuilder.RenameColumn(
                name: "Uz_MetaKeywords",
                table: "Products",
                newName: "MetaKeywords");

            migrationBuilder.RenameColumn(
                name: "Uz_MetaDescription",
                table: "Products",
                newName: "MetaDescription");

            migrationBuilder.RenameColumn(
                name: "Uz_Description",
                table: "Products",
                newName: "Description");
        }
    }
}
