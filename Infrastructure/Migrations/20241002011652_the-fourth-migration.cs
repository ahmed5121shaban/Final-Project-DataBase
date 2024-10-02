using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class thefourthmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                    { "07dc6e70-6405-480a-a373-7583f750499a", null, "User", "USER" },
                    { "18c65294-5d21-47aa-9dcf-a5eb0ee1e82c", null, "Seller", "SELLER" },
                    { "7055ecd9-2c51-4e2f-8772-6c909eec00c7", null, "Buyer", "BUYER" },
                    { "d7d75893-60b2-4334-8a66-f339b819c48a", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "07dc6e70-6405-480a-a373-7583f750499a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "18c65294-5d21-47aa-9dcf-a5eb0ee1e82c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7055ecd9-2c51-4e2f-8772-6c909eec00c7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d7d75893-60b2-4334-8a66-f339b819c48a");

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
    }
}
