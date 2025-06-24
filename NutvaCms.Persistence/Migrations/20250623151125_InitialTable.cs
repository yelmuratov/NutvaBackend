using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NutvaCms.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    IsSuperAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    En_Title = table.Column<string>(type: "text", nullable: false),
                    En_Subtitle = table.Column<string>(type: "text", nullable: false),
                    En_MetaTitle = table.Column<string>(type: "text", nullable: false),
                    En_MetaDescription = table.Column<string>(type: "text", nullable: false),
                    En_MetaKeywords = table.Column<string>(type: "text", nullable: false),
                    Uz_Title = table.Column<string>(type: "text", nullable: false),
                    Uz_Subtitle = table.Column<string>(type: "text", nullable: false),
                    Uz_MetaTitle = table.Column<string>(type: "text", nullable: false),
                    Uz_MetaDescription = table.Column<string>(type: "text", nullable: false),
                    Uz_MetaKeywords = table.Column<string>(type: "text", nullable: false),
                    Ru_Title = table.Column<string>(type: "text", nullable: false),
                    Ru_Subtitle = table.Column<string>(type: "text", nullable: false),
                    Ru_MetaTitle = table.Column<string>(type: "text", nullable: false),
                    Ru_MetaDescription = table.Column<string>(type: "text", nullable: false),
                    Ru_MetaKeywords = table.Column<string>(type: "text", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: false),
                    ImageUrls = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.Id);
                });

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
                name: "ChatAdmins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    TelegramUserId = table.Column<long>(type: "bigint", nullable: false),
                    IsBusy = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatAdmins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    En_Name = table.Column<string>(type: "text", nullable: false),
                    En_Description = table.Column<string>(type: "text", nullable: false),
                    En_MetaTitle = table.Column<string>(type: "text", nullable: false),
                    En_MetaDescription = table.Column<string>(type: "text", nullable: false),
                    En_MetaKeywords = table.Column<string>(type: "text", nullable: false),
                    Uz_Name = table.Column<string>(type: "text", nullable: false),
                    Uz_Description = table.Column<string>(type: "text", nullable: false),
                    Uz_MetaTitle = table.Column<string>(type: "text", nullable: false),
                    Uz_MetaDescription = table.Column<string>(type: "text", nullable: false),
                    Uz_MetaKeywords = table.Column<string>(type: "text", nullable: false),
                    Ru_Name = table.Column<string>(type: "text", nullable: false),
                    Ru_Description = table.Column<string>(type: "text", nullable: false),
                    Ru_MetaTitle = table.Column<string>(type: "text", nullable: false),
                    Ru_MetaDescription = table.Column<string>(type: "text", nullable: false),
                    Ru_MetaKeywords = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    BuyClickCount = table.Column<int>(type: "integer", nullable: false),
                    ImageUrls = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteStatistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalVisits = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteStatistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrackingPixels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Platform = table.Column<string>(type: "text", nullable: false),
                    Script = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingPixels", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "ChatSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChatAdminId = table.Column<int>(type: "integer", nullable: true),
                    UserIdentifier = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatSessions_ChatAdmins_ChatAdminId",
                        column: x => x.ChatAdminId,
                        principalTable: "ChatAdmins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerName = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseRequests_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChatSessionId = table.Column<int>(type: "integer", nullable: false),
                    Sender = table.Column<string>(type: "text", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatSessions_ChatSessionId",
                        column: x => x.ChatSessionId,
                        principalTable: "ChatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostMedia_BlogPostId",
                table: "BlogPostMedia",
                column: "BlogPostId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ChatSessionId",
                table: "ChatMessages",
                column: "ChatSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatSessions_ChatAdminId",
                table: "ChatSessions",
                column: "ChatAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_ProductId",
                table: "PurchaseRequests",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Banners");

            migrationBuilder.DropTable(
                name: "BlogPostMedia");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "PurchaseRequests");

            migrationBuilder.DropTable(
                name: "SiteStatistics");

            migrationBuilder.DropTable(
                name: "TrackingPixels");

            migrationBuilder.DropTable(
                name: "BlogPosts");

            migrationBuilder.DropTable(
                name: "ChatSessions");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ChatAdmins");
        }
    }
}
