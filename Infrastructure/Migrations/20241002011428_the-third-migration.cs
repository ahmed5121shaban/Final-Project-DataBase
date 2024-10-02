using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class thethirdmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "08ce20c1-83dc-4c1f-8a91-f5d8a82fbf4e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "78b84c00-0fab-40ea-b85a-e6723ab77a8e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8b1c3db8-3263-4981-8b9e-47bc5309c6d5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d68980f8-568f-4922-85f1-2b5e0cad7171");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8493032c-d4c8-4ceb-899a-51e30dde1035", null, "Admin", "ADMIN" },
                    { "a6c04a09-ff09-4069-b5f1-dc1676b9d531", null, "Buyer", "BUYER" },
                    { "b00e504f-16df-4284-92d5-fc9bfcb006ea", null, "Seller", "SELLER" },
                    { "ebdd7ded-b04b-4d67-bf10-0181ac8b7b02", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8493032c-d4c8-4ceb-899a-51e30dde1035");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a6c04a09-ff09-4069-b5f1-dc1676b9d531");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b00e504f-16df-4284-92d5-fc9bfcb006ea");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ebdd7ded-b04b-4d67-bf10-0181ac8b7b02");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "08ce20c1-83dc-4c1f-8a91-f5d8a82fbf4e", null, "Seller", "SELLER" },
                    { "78b84c00-0fab-40ea-b85a-e6723ab77a8e", null, "User", "USER" },
                    { "8b1c3db8-3263-4981-8b9e-47bc5309c6d5", null, "Admin", "ADMIN" },
                    { "d68980f8-568f-4922-85f1-2b5e0cad7171", null, "Buyer", "BUYER" }
                });
        }
    }
}
