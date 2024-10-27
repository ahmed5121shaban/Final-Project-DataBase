using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ahmed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0fdfc9e6-1a63-47a0-8dfc-4163d1a04f3b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a2c09704-7ce8-4142-a4e2-4c708032fb02");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d36d99ef-8c53-4a65-a511-5b9edf61926b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fd6e974a-c5b8-43f6-99f5-8ee05ee13c15");

            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "Message",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4dcc8456-db83-4031-9757-c4f506fbd39a", null, "Admin", "ADMIN" },
                    { "524cd8e0-199f-4b2d-99a7-08c1334f6997", null, "User", "USER" },
                    { "c444ed47-8867-4fc1-b1f9-9692df004f6f", null, "Buyer", "BUYER" },
                    { "e587cadc-0c95-42c3-abb5-6d6d0e9b7577", null, "Seller", "SELLER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Message_UserID",
                table: "Message",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_AspNetUsers_UserID",
                table: "Message",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_AspNetUsers_UserID",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_UserID",
                table: "Message");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4dcc8456-db83-4031-9757-c4f506fbd39a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "524cd8e0-199f-4b2d-99a7-08c1334f6997");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c444ed47-8867-4fc1-b1f9-9692df004f6f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e587cadc-0c95-42c3-abb5-6d6d0e9b7577");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Message");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0fdfc9e6-1a63-47a0-8dfc-4163d1a04f3b", null, "User", "USER" },
                    { "a2c09704-7ce8-4142-a4e2-4c708032fb02", null, "Seller", "SELLER" },
                    { "d36d99ef-8c53-4a65-a511-5b9edf61926b", null, "Buyer", "BUYER" },
                    { "fd6e974a-c5b8-43f6-99f5-8ee05ee13c15", null, "Admin", "ADMIN" }
                });
        }
    }
}
