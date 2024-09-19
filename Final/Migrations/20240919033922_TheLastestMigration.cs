using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final.Migrations
{
    /// <inheritdoc />
    public partial class TheLastestMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SavedCategoryID",
                table: "Buyer");

            migrationBuilder.DropColumn(
                name: "SavedItemID",
                table: "Buyer");

            migrationBuilder.AddColumn<int>(
                name: "AuctionID",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_ItemID",
                table: "Auctions",
                column: "ItemID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Items_ItemID",
                table: "Auctions",
                column: "ItemID",
                principalTable: "Items",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Items_ItemID",
                table: "Auctions");

            migrationBuilder.DropIndex(
                name: "IX_Auctions_ItemID",
                table: "Auctions");

            migrationBuilder.DropColumn(
                name: "AuctionID",
                table: "Items");

            migrationBuilder.AddColumn<int>(
                name: "SavedCategoryID",
                table: "Buyer",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SavedItemID",
                table: "Buyer",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
