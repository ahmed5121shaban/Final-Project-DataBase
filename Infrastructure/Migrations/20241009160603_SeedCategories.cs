using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4785f413-c2b0-4cb6-a9c3-2997fb87ca85");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8327cc2f-b61c-4a49-9f0d-526feee23e75");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d8ba2570-8b4c-4f7b-8ead-d5a17ed7d6d8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e2a45779-2e5f-4257-beca-44458dacbff9");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2b2ac735-ed10-4373-a814-5792b291c50d", null, "Seller", "SELLER" },
                    { "3b7f6258-d8d2-4fac-9a33-c7ec13b42384", null, "Buyer", "BUYER" },
                    { "418baf7a-6a0b-4644-94f9-a73df0103440", null, "Admin", "ADMIN" },
                    { "6990afe6-8eaa-4642-b03d-347790195775", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "ID", "Description", "Image", "Name" },
                values: new object[,]
                {
                    { 1, "Description scripe scripe scripe scripe scripe scripe scripe", "https://picsum.photos/seed/picsum/214/300", "Cars" },
                    { 2, "Description scripe scripe scripe scripe scripe scripe scripe", "https://picsum.photos/seed/picsum/213/300", "Food" },
                    { 3, "Description scripe scripe scripe scripe scripe scripe scripe", "https://picsum.photos/seed/picsum/212/300", "Electronic" },
                    { 4, "Description scripe scripe scripe scripe scripe scripe scripe", "https://picsum.photos/seed/picsum/211/300", "Cloths" },
                    { 5, "Description scripe scripe scripe scripe scripe scripe scripe", "https://picsum.photos/seed/picsum/210/300", "Toy" },
                    { 6, "Description scripe scripe scripe scripe scripe scripe scripe", "https://picsum.photos/seed/picsum/201/300", "Others" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2b2ac735-ed10-4373-a814-5792b291c50d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3b7f6258-d8d2-4fac-9a33-c7ec13b42384");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "418baf7a-6a0b-4644-94f9-a73df0103440");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6990afe6-8eaa-4642-b03d-347790195775");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 6);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4785f413-c2b0-4cb6-a9c3-2997fb87ca85", null, "Admin", "ADMIN" },
                    { "8327cc2f-b61c-4a49-9f0d-526feee23e75", null, "Buyer", "BUYER" },
                    { "d8ba2570-8b4c-4f7b-8ead-d5a17ed7d6d8", null, "Seller", "SELLER" },
                    { "e2a45779-2e5f-4257-beca-44458dacbff9", null, "User", "USER" }
                });
        }
    }
}
