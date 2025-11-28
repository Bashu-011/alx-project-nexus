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

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("85ba05f7-9ec0-4f46-b312-8292a0ad3740"), new DateTime(2025, 11, 27, 13, 10, 25, 126, DateTimeKind.Utc).AddTicks(3415), "Books and magazines", "Books", "books", new DateTime(2025, 11, 27, 13, 10, 25, 126, DateTimeKind.Utc).AddTicks(3415) },
                    { new Guid("e0a41bfc-8187-479a-96da-82becdbdc2e2"), new DateTime(2025, 11, 27, 13, 10, 25, 126, DateTimeKind.Utc).AddTicks(3411), "Electronic devices and gadgets", "Electronics", "electronics", new DateTime(2025, 11, 27, 13, 10, 25, 126, DateTimeKind.Utc).AddTicks(3413) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("85ba05f7-9ec0-4f46-b312-8292a0ad3740"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("e0a41bfc-8187-479a-96da-82becdbdc2e2"));

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
