using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCartFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("41b6a53d-c6cc-4912-bce6-e82391814336"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("a73b4d8d-e743-4252-a47f-bc1ef484cd27"));

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0ab5a00a-c59c-4e7e-a38a-0ca0f13670a9"), new DateTime(2025, 11, 28, 10, 37, 20, 49, DateTimeKind.Utc).AddTicks(609), "Books and magazines", "Books", "books", new DateTime(2025, 11, 28, 10, 37, 20, 49, DateTimeKind.Utc).AddTicks(609) },
                    { new Guid("b36939cc-b795-49ee-8f09-641266face46"), new DateTime(2025, 11, 28, 10, 37, 20, 49, DateTimeKind.Utc).AddTicks(601), "Electronic devices and gadgets", "Electronics", "electronics", new DateTime(2025, 11, 28, 10, 37, 20, 49, DateTimeKind.Utc).AddTicks(607) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { new Guid("41b6a53d-c6cc-4912-bce6-e82391814336"), new DateTime(2025, 11, 28, 10, 35, 26, 980, DateTimeKind.Utc).AddTicks(7405), "Electronic devices and gadgets", "Electronics", "electronics", new DateTime(2025, 11, 28, 10, 35, 26, 980, DateTimeKind.Utc).AddTicks(7408) },
                    { new Guid("a73b4d8d-e743-4252-a47f-bc1ef484cd27"), new DateTime(2025, 11, 28, 10, 35, 26, 980, DateTimeKind.Utc).AddTicks(7409), "Books and magazines", "Books", "books", new DateTime(2025, 11, 28, 10, 35, 26, 980, DateTimeKind.Utc).AddTicks(7409) }
                });
        }
    }
}
