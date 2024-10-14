using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ahmedthree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "PaymentEmail",
                table: "Bids");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "126f8294-f1fd-4a8e-9d7c-6e23bd2f633e", null, "Buyer", "BUYER" },
                    { "1993bf80-df53-4730-83ae-9b7cf048d72f", null, "Admin", "ADMIN" },
                    { "46868fea-1ed3-42b3-ba65-4374e53e09d0", null, "Seller", "SELLER" },
                    { "5efa2547-3ecc-4f97-b1da-4f64f37efac4", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "126f8294-f1fd-4a8e-9d7c-6e23bd2f633e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1993bf80-df53-4730-83ae-9b7cf048d72f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "46868fea-1ed3-42b3-ba65-4374e53e09d0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5efa2547-3ecc-4f97-b1da-4f64f37efac4");

            migrationBuilder.AddColumn<string>(
                name: "PaymentEmail",
                table: "Bids",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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
    }
}
