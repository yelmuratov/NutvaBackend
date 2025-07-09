using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutvaCms.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePurchaseRequestEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "ForWhom",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "Problem",
                table: "PurchaseRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "PurchaseRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ForWhom",
                table: "PurchaseRequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Problem",
                table: "PurchaseRequests",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
