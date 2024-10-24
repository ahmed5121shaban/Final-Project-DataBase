using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class newmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2b1bb2e7-d4ef-4538-b840-5b838c771bb1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6a0f68f3-1a65-4495-a635-d2a55e2770b5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a4388a75-e1a2-401e-9d86-a55a8f179150");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bf66aad9-f6f3-494a-b4f0-dc7e52b23978");

            migrationBuilder.AddColumn<decimal>(
                name: "WithdrawnAmount",
                table: "Seller",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0bc154c0-8a12-4e7b-b1b8-16e6339ea905", null, "Admin", "ADMIN" },
                    { "4bd2a72b-295d-4953-9853-d39cf2e1fc52", null, "User", "USER" },
                    { "5290368c-0539-4ecf-9b91-757652cb6b09", null, "Seller", "SELLER" },
                    { "a970be32-0b8a-4163-9755-f9f11930298d", null, "Buyer", "BUYER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0bc154c0-8a12-4e7b-b1b8-16e6339ea905");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4bd2a72b-295d-4953-9853-d39cf2e1fc52");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5290368c-0539-4ecf-9b91-757652cb6b09");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a970be32-0b8a-4163-9755-f9f11930298d");

            migrationBuilder.DropColumn(
                name: "WithdrawnAmount",
                table: "Seller");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2b1bb2e7-d4ef-4538-b840-5b838c771bb1", null, "Buyer", "BUYER" },
                    { "6a0f68f3-1a65-4495-a635-d2a55e2770b5", null, "Seller", "SELLER" },
                    { "a4388a75-e1a2-401e-9d86-a55a8f179150", null, "User", "USER" },
                    { "bf66aad9-f6f3-494a-b4f0-dc7e52b23978", null, "Admin", "ADMIN" }
                });
        }
    }
}
