using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutvaCms.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blogs");

            migrationBuilder.CreateTable(
                name: "BlogPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Published = table.Column<bool>(type: "boolean", nullable: false),
                    En_Title = table.Column<string>(type: "text", nullable: false),
                    En_Subtitle = table.Column<string>(type: "text", nullable: true),
                    En_Content = table.Column<string>(type: "text", nullable: false),
                    En_MetaTitle = table.Column<string>(type: "text", nullable: false),
                    En_MetaDescription = table.Column<string>(type: "text", nullable: false),
                    En_MetaKeywords = table.Column<string>(type: "text", nullable: false),
                    Uz_Title = table.Column<string>(type: "text", nullable: false),
                    Uz_Subtitle = table.Column<string>(type: "text", nullable: true),
                    Uz_Content = table.Column<string>(type: "text", nullable: false),
                    Uz_MetaTitle = table.Column<string>(type: "text", nullable: false),
                    Uz_MetaDescription = table.Column<string>(type: "text", nullable: false),
                    Uz_MetaKeywords = table.Column<string>(type: "text", nullable: false),
                    Ru_Title = table.Column<string>(type: "text", nullable: false),
                    Ru_Subtitle = table.Column<string>(type: "text", nullable: true),
                    Ru_Content = table.Column<string>(type: "text", nullable: false),
                    Ru_MetaTitle = table.Column<string>(type: "text", nullable: false),
                    Ru_MetaDescription = table.Column<string>(type: "text", nullable: false),
                    Ru_MetaKeywords = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogPostMedia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BlogPostId = table.Column<Guid>(type: "uuid", nullable: false),
                    MediaType = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Caption = table.Column<string>(type: "text", nullable: true),
                    AltText = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPostMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogPostMedia_BlogPosts_BlogPostId",
                        column: x => x.BlogPostId,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostMedia_BlogPostId",
                table: "BlogPostMedia",
                column: "BlogPostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogPostMedia");

            migrationBuilder.DropTable(
                name: "BlogPosts");

            migrationBuilder.CreateTable(
                name: "Blogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ImageUrls = table.Column<string>(type: "text", nullable: false),
                    MetaDescription = table.Column<string>(type: "text", nullable: false),
                    MetaKeywords = table.Column<string>(type: "text", nullable: false),
                    MetaTitle = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    ViewCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blogs", x => x.Id);
                });
        }
    }
}
