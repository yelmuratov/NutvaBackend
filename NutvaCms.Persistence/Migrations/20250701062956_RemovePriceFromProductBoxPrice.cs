using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutvaCms.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovePriceFromProductBoxPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "ProductBoxPrices");

            migrationBuilder.AlterColumn<string>(
                name: "DiscountLabel",
                table: "ProductBoxPrices",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DiscountLabel",
                table: "ProductBoxPrices",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ProductBoxPrices",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
