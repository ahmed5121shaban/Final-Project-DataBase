using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addingpayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3eba764c-914e-468a-9fda-1e22d4313f49");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4a0d0c8c-aee7-40d5-8fb8-ffd7d7e4c101");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c439a3a6-6069-418a-a820-f6bb17f49ecc");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cb54b2fc-329c-46a3-89d9-568170112d46");

            migrationBuilder.RenameColumn(
                name: "PaymantEmail",
                table: "AspNetUsers",
                newName: "StripeEmail");

            migrationBuilder.AddColumn<string>(
                name: "PaymentEmail",
                table: "Bids",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaypalEmail",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "PaymentEmail",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "PaypalEmail",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "StripeEmail",
                table: "AspNetUsers",
                newName: "PaymantEmail");

            migrationBuilder.AlterColumn<int>(
                name: "NationalId",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3eba764c-914e-468a-9fda-1e22d4313f49", null, "Admin", "ADMIN" },
                    { "4a0d0c8c-aee7-40d5-8fb8-ffd7d7e4c101", null, "User", "USER" },
                    { "c439a3a6-6069-418a-a820-f6bb17f49ecc", null, "Seller", "SELLER" },
                    { "cb54b2fc-329c-46a3-89d9-568170112d46", null, "Buyer", "BUYER" }
                });
        }
    }
}
