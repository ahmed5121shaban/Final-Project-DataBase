using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final.Migrations
{
    /// <inheritdoc />
    public partial class InitialsCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_UserID",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Auctions_AuctionID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_AuctionID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AuctionID",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Chats",
                newName: "SellerID");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_UserID",
                table: "Chats",
                newName: "IX_Chats_SellerID");

            migrationBuilder.AddColumn<int>(
                name: "BuyerID",
                table: "Chats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "Auctions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_BuyerID",
                table: "Chats",
                column: "BuyerID");

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_UserID",
                table: "Auctions",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Users_UserID",
                table: "Auctions",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_BuyerID",
                table: "Chats",
                column: "BuyerID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_SellerID",
                table: "Chats",
                column: "SellerID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Users_UserID",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_BuyerID",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_SellerID",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_BuyerID",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Auctions_UserID",
                table: "Auctions");

            migrationBuilder.DropColumn(
                name: "BuyerID",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "Completed",
                table: "Auctions");

            migrationBuilder.RenameColumn(
                name: "SellerID",
                table: "Chats",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_SellerID",
                table: "Chats",
                newName: "IX_Chats_UserID");

            migrationBuilder.AddColumn<int>(
                name: "AuctionID",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_AuctionID",
                table: "Users",
                column: "AuctionID");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_UserID",
                table: "Chats",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Auctions_AuctionID",
                table: "Users",
                column: "AuctionID",
                principalTable: "Auctions",
                principalColumn: "ID");
        }
    }
}
