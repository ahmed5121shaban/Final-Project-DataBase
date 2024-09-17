using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final.Migrations
{
    /// <inheritdoc />
    public partial class newMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "BuyerCategory",
                columns: table => new
                {
                    BuyersID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SavedCategoriesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerCategory", x => new { x.BuyersID, x.SavedCategoriesID });
                    table.ForeignKey(
                        name: "FK_BuyerCategory_Buyer_BuyersID",
                        column: x => x.BuyersID,
                        principalTable: "Buyer",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuyerCategory_Categories_SavedCategoriesID",
                        column: x => x.SavedCategoriesID,
                        principalTable: "Categories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuyerItem",
                columns: table => new
                {
                    BuyersID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SavedItemsID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerItem", x => new { x.BuyersID, x.SavedItemsID });
                    table.ForeignKey(
                        name: "FK_BuyerItem_Buyer_BuyersID",
                        column: x => x.BuyersID,
                        principalTable: "Buyer",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuyerItem_Items_SavedItemsID",
                        column: x => x.SavedItemsID,
                        principalTable: "Items",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerCategory_SavedCategoriesID",
                table: "BuyerCategory",
                column: "SavedCategoriesID");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerItem_SavedItemsID",
                table: "BuyerItem",
                column: "SavedItemsID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuyerCategory");

            migrationBuilder.DropTable(
                name: "BuyerItem");

            migrationBuilder.DropColumn(
                name: "SavedCategoryID",
                table: "Buyer");

            migrationBuilder.DropColumn(
                name: "SavedItemID",
                table: "Buyer");
        }
    }
}
