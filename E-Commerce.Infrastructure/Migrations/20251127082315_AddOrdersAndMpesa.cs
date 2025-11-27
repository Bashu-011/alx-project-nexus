using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdersAndMpesa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("9e2d54b2-77e3-465a-9686-2bae3525ea31"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("d5cbf343-80b9-4bc9-8261-4dd0f11e9162"));

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    MpesaReceiptNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MpesaCheckoutRequestId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    MpesaMerchantRequestId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProductImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("947224f8-b33d-46ab-bea8-ea7d58794e2e"), new DateTime(2025, 11, 27, 8, 23, 14, 721, DateTimeKind.Utc).AddTicks(557), "Electronic devices and gadgets", "Electronics", "electronics", new DateTime(2025, 11, 27, 8, 23, 14, 721, DateTimeKind.Utc).AddTicks(559) },
                    { new Guid("c2ebe59d-b493-4730-acb5-cb60f298eb52"), new DateTime(2025, 11, 27, 8, 23, 14, 721, DateTimeKind.Utc).AddTicks(561), "Books and magazines", "Books", "books", new DateTime(2025, 11, 27, 8, 23, 14, 721, DateTimeKind.Utc).AddTicks(561) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_MpesaCheckoutRequestId",
                table: "Orders",
                column: "MpesaCheckoutRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("947224f8-b33d-46ab-bea8-ea7d58794e2e"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("c2ebe59d-b493-4730-acb5-cb60f298eb52"));

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("9e2d54b2-77e3-465a-9686-2bae3525ea31"), new DateTime(2025, 11, 27, 5, 40, 40, 93, DateTimeKind.Utc).AddTicks(1313), "Books and magazines", "Books", "books", new DateTime(2025, 11, 27, 5, 40, 40, 93, DateTimeKind.Utc).AddTicks(1313) },
                    { new Guid("d5cbf343-80b9-4bc9-8261-4dd0f11e9162"), new DateTime(2025, 11, 27, 5, 40, 40, 93, DateTimeKind.Utc).AddTicks(1309), "Electronic devices and gadgets", "Electronics", "electronics", new DateTime(2025, 11, 27, 5, 40, 40, 93, DateTimeKind.Utc).AddTicks(1311) }
                });
        }
    }
}
