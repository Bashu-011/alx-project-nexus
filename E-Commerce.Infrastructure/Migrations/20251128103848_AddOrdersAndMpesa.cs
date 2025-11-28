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
                keyValue: new Guid("0ab5a00a-c59c-4e7e-a38a-0ca0f13670a9"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b36939cc-b795-49ee-8f09-641266face46"));

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("5046fdb5-e830-41cc-acc3-ed26e80f55b3"), new DateTime(2025, 11, 28, 10, 38, 47, 826, DateTimeKind.Utc).AddTicks(2581), "Electronic devices and gadgets", "Electronics", "electronics", new DateTime(2025, 11, 28, 10, 38, 47, 826, DateTimeKind.Utc).AddTicks(2586) },
                    { new Guid("beeedd38-e7d2-4733-9680-b75d12c9075d"), new DateTime(2025, 11, 28, 10, 38, 47, 826, DateTimeKind.Utc).AddTicks(2588), "Books and magazines", "Books", "books", new DateTime(2025, 11, 28, 10, 38, 47, 826, DateTimeKind.Utc).AddTicks(2588) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("5046fdb5-e830-41cc-acc3-ed26e80f55b3"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("beeedd38-e7d2-4733-9680-b75d12c9075d"));

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0ab5a00a-c59c-4e7e-a38a-0ca0f13670a9"), new DateTime(2025, 11, 28, 10, 37, 20, 49, DateTimeKind.Utc).AddTicks(609), "Books and magazines", "Books", "books", new DateTime(2025, 11, 28, 10, 37, 20, 49, DateTimeKind.Utc).AddTicks(609) },
                    { new Guid("b36939cc-b795-49ee-8f09-641266face46"), new DateTime(2025, 11, 28, 10, 37, 20, 49, DateTimeKind.Utc).AddTicks(601), "Electronic devices and gadgets", "Electronics", "electronics", new DateTime(2025, 11, 28, 10, 37, 20, 49, DateTimeKind.Utc).AddTicks(607) }
                });
        }
    }
}
