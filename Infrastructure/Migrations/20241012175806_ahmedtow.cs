using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ahmedtow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "01fdb9df-c901-4cc8-8066-c595f937de4c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4b7668b4-67c4-4b07-a008-d5a403005061");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9d7287e5-4980-455d-85c0-07aa3af02a3d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b80f1f72-c75c-4322-bf99-49c481cff883");

            migrationBuilder.AlterColumn<int>(
                name: "AuctionID",
                table: "Payment",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6a1689b3-7c20-4aa4-bff6-391c95408405", null, "User", "USER" },
                    { "9007bd55-50da-4398-aafa-44c727519ca0", null, "Seller", "SELLER" },
                    { "a1c29856-4c8a-441b-b731-48f260c4a47c", null, "Admin", "ADMIN" },
                    { "f30d73bf-5675-4ebf-889b-132080ca6956", null, "Buyer", "BUYER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6a1689b3-7c20-4aa4-bff6-391c95408405");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9007bd55-50da-4398-aafa-44c727519ca0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a1c29856-4c8a-441b-b731-48f260c4a47c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f30d73bf-5675-4ebf-889b-132080ca6956");

            migrationBuilder.AlterColumn<int>(
                name: "AuctionID",
                table: "Payment",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "01fdb9df-c901-4cc8-8066-c595f937de4c", null, "User", "USER" },
                    { "4b7668b4-67c4-4b07-a008-d5a403005061", null, "Admin", "ADMIN" },
                    { "9d7287e5-4980-455d-85c0-07aa3af02a3d", null, "Seller", "SELLER" },
                    { "b80f1f72-c75c-4322-bf99-49c481cff883", null, "Buyer", "BUYER" }
                });
        }
    }
}
