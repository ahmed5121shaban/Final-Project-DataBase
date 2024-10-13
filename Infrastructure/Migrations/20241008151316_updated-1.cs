using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updated1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Buyer_BuyerID",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Payment_PaymentID",
                table: "Auctions");

            migrationBuilder.DropIndex(
                name: "IX_Auctions_PaymentID",
                table: "Auctions");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "005a6ca6-3d61-46d8-8592-74fb9c7adfd1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4aaf8baa-5e9b-42a7-a13e-0ec0a0cab8ee");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5a55878d-612b-4250-962b-9b9b9d659583");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5a61e5ca-b524-4b0a-8b49-5c2de0a37347");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentID",
                table: "Auctions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "BuyerID",
                table: "Auctions",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

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

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_PaymentID",
                table: "Auctions",
                column: "PaymentID",
                unique: true,
                filter: "[PaymentID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Buyer_BuyerID",
                table: "Auctions",
                column: "BuyerID",
                principalTable: "Buyer",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Payment_PaymentID",
                table: "Auctions",
                column: "PaymentID",
                principalTable: "Payment",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Buyer_BuyerID",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Payment_PaymentID",
                table: "Auctions");

            migrationBuilder.DropIndex(
                name: "IX_Auctions_PaymentID",
                table: "Auctions");

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

            migrationBuilder.AlterColumn<int>(
                name: "PaymentID",
                table: "Auctions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BuyerID",
                table: "Auctions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "005a6ca6-3d61-46d8-8592-74fb9c7adfd1", null, "Admin", "ADMIN" },
                    { "4aaf8baa-5e9b-42a7-a13e-0ec0a0cab8ee", null, "Buyer", "BUYER" },
                    { "5a55878d-612b-4250-962b-9b9b9d659583", null, "Seller", "SELLER" },
                    { "5a61e5ca-b524-4b0a-8b49-5c2de0a37347", null, "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_PaymentID",
                table: "Auctions",
                column: "PaymentID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Buyer_BuyerID",
                table: "Auctions",
                column: "BuyerID",
                principalTable: "Buyer",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Payment_PaymentID",
                table: "Auctions",
                column: "PaymentID",
                principalTable: "Payment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
