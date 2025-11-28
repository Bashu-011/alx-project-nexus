using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("0c94cc95-b35c-4a5f-bb7b-85d3ba15c5ee"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("73b2e2bd-77cc-40e8-9966-68e8ed0eb982"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0c94cc95-b35c-4a5f-bb7b-85d3ba15c5ee"), new DateTime(2025, 11, 28, 9, 19, 16, 778, DateTimeKind.Utc).AddTicks(1700), "Books and magazines", "Books", "books", new DateTime(2025, 11, 28, 9, 19, 16, 778, DateTimeKind.Utc).AddTicks(1700) },
                    { new Guid("73b2e2bd-77cc-40e8-9966-68e8ed0eb982"), new DateTime(2025, 11, 28, 9, 19, 16, 778, DateTimeKind.Utc).AddTicks(1697), "Electronic devices and gadgets", "Electronics", "electronics", new DateTime(2025, 11, 28, 9, 19, 16, 778, DateTimeKind.Utc).AddTicks(1698) }
                });
        }
    }
}
