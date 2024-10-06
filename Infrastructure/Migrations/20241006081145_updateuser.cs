using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_AspNetUsers_UserID",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Bids_AspNetUsers_UserID",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_BuyerCategory_Buyer_BuyersID",
                table: "BuyerCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_AspNetUsers_BuyerID",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_AspNetUsers_SellerID",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_AspNetUsers_UserID",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Review_ReviewID",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_AspNetUsers_UserId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_AspNetUsers_UserID",
                table: "Review");

            migrationBuilder.DropTable(
                name: "BuyerItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Seller",
                table: "Seller");

            migrationBuilder.DropIndex(
                name: "IX_Seller_UserID",
                table: "Seller");

            migrationBuilder.DropIndex(
                name: "IX_Items_ReviewID",
                table: "Items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Buyer",
                table: "Buyer");

            migrationBuilder.DropIndex(
                name: "IX_Buyer_UserID",
                table: "Buyer");

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

            migrationBuilder.DropColumn(
                name: "ID",
                table: "Seller");

            migrationBuilder.DropColumn(
                name: "ItemID",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "ReviewID",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "Buyer");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Review",
                newName: "SellerID");

            migrationBuilder.RenameColumn(
                name: "Descrip",
                table: "Review",
                newName: "Description");

            migrationBuilder.RenameIndex(
                name: "IX_Review_UserID",
                table: "Review",
                newName: "IX_Review_SellerID");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Payment",
                newName: "BuyerId");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_UserId",
                table: "Payment",
                newName: "IX_Payment_BuyerId");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Items",
                newName: "SellerID");

            migrationBuilder.RenameIndex(
                name: "IX_Items_UserID",
                table: "Items",
                newName: "IX_Items_SellerID");

            migrationBuilder.RenameColumn(
                name: "BuyersID",
                table: "BuyerCategory",
                newName: "BuyersUserID");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Bids",
                newName: "BuyerID");

            migrationBuilder.RenameIndex(
                name: "IX_Bids_UserID",
                table: "Bids",
                newName: "IX_Bids_BuyerID");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Auctions",
                newName: "BuyerID");

            migrationBuilder.RenameIndex(
                name: "IX_Auctions_UserID",
                table: "Auctions",
                newName: "IX_Auctions_BuyerID");

            migrationBuilder.AddColumn<string>(
                name: "BuyerID",
                table: "Review",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "AuctionID",
                table: "Items",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "BuyerUserID",
                table: "Items",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NationalId",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Gender",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<int>(
                name: "Age",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "PaymantEmail",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Seller",
                table: "Seller",
                column: "UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Buyer",
                table: "Buyer",
                column: "UserID");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "373be9da-4eac-4ab5-9f80-88d6aac10605", null, "Seller", "SELLER" },
                    { "3ceecf5d-a462-42b4-a6f3-d5f13172e7f0", null, "User", "USER" },
                    { "66bcce4f-ecd1-4908-bd79-4416e2b7f741", null, "Buyer", "BUYER" },
                    { "ec2d9381-e743-4345-8b74-e82ef97bb7fb", null, "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Review_BuyerID",
                table: "Review",
                column: "BuyerID");

            migrationBuilder.CreateIndex(
                name: "IX_Items_BuyerUserID",
                table: "Items",
                column: "BuyerUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Buyer_BuyerID",
                table: "Auctions",
                column: "BuyerID",
                principalTable: "Buyer",
                principalColumn: "UserID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Buyer_BuyerID",
                table: "Bids",
                column: "BuyerID",
                principalTable: "Buyer",
                principalColumn: "UserID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_BuyerCategory_Buyer_BuyersUserID",
                table: "BuyerCategory",
                column: "BuyersUserID",
                principalTable: "Buyer",
                principalColumn: "UserID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Buyer_BuyerID",
                table: "Chats",
                column: "BuyerID",
                principalTable: "Buyer",
                principalColumn: "UserID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Seller_SellerID",
                table: "Chats",
                column: "SellerID",
                principalTable: "Seller",
                principalColumn: "UserID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Buyer_BuyerUserID",
                table: "Items",
                column: "BuyerUserID",
                principalTable: "Buyer",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Seller_SellerID",
                table: "Items",
                column: "SellerID",
                principalTable: "Seller",
                principalColumn: "UserID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Buyer_BuyerId",
                table: "Payment",
                column: "BuyerId",
                principalTable: "Buyer",
                principalColumn: "UserID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Buyer_BuyerID",
                table: "Review",
                column: "BuyerID",
                principalTable: "Buyer",
                principalColumn: "UserID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Seller_SellerID",
                table: "Review",
                column: "SellerID",
                principalTable: "Seller",
                principalColumn: "UserID",
                onDelete: ReferentialAction.NoAction);
            migrationBuilder.DropColumn(
                name: "IsReviewed",
                table: "Items");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Buyer_BuyerID",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Buyer_BuyerID",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_BuyerCategory_Buyer_BuyersUserID",
                table: "BuyerCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Buyer_BuyerID",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Seller_SellerID",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Buyer_BuyerUserID",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Seller_SellerID",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Buyer_BuyerId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Buyer_BuyerID",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Seller_SellerID",
                table: "Review");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Seller",
                table: "Seller");

            migrationBuilder.DropIndex(
                name: "IX_Review_BuyerID",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Items_BuyerUserID",
                table: "Items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Buyer",
                table: "Buyer");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "373be9da-4eac-4ab5-9f80-88d6aac10605");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3ceecf5d-a462-42b4-a6f3-d5f13172e7f0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "66bcce4f-ecd1-4908-bd79-4416e2b7f741");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ec2d9381-e743-4345-8b74-e82ef97bb7fb");

            migrationBuilder.DropColumn(
                name: "BuyerID",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "BuyerUserID",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "PaymantEmail",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "SellerID",
                table: "Review",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Review",
                newName: "Descrip");

            migrationBuilder.RenameIndex(
                name: "IX_Review_SellerID",
                table: "Review",
                newName: "IX_Review_UserID");

            migrationBuilder.RenameColumn(
                name: "BuyerId",
                table: "Payment",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_BuyerId",
                table: "Payment",
                newName: "IX_Payment_UserId");

            migrationBuilder.RenameColumn(
                name: "SellerID",
                table: "Items",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Items_SellerID",
                table: "Items",
                newName: "IX_Items_UserID");

            migrationBuilder.RenameColumn(
                name: "BuyersUserID",
                table: "BuyerCategory",
                newName: "BuyersID");

            migrationBuilder.RenameColumn(
                name: "BuyerID",
                table: "Bids",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Bids_BuyerID",
                table: "Bids",
                newName: "IX_Bids_UserID");

            migrationBuilder.RenameColumn(
                name: "BuyerID",
                table: "Auctions",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Auctions_BuyerID",
                table: "Auctions",
                newName: "IX_Auctions_UserID");

            migrationBuilder.AddColumn<string>(
                name: "ID",
                table: "Seller",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ItemID",
                table: "Review",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "AuctionID",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewID",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ID",
                table: "Buyer",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "NationalId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Age",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Seller",
                table: "Seller",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Buyer",
                table: "Buyer",
                column: "ID");

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
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_BuyerItem_Items_SavedItemsID",
                        column: x => x.SavedItemsID,
                        principalTable: "Items",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Seller_UserID",
                table: "Seller",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_ReviewID",
                table: "Items",
                column: "ReviewID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Buyer_UserID",
                table: "Buyer",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyerItem_SavedItemsID",
                table: "BuyerItem",
                column: "SavedItemsID");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_AspNetUsers_UserID",
                table: "Auctions",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_AspNetUsers_UserID",
                table: "Bids",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_BuyerCategory_Buyer_BuyersID",
                table: "BuyerCategory",
                column: "BuyersID",
                principalTable: "Buyer",
                principalColumn: "ID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_AspNetUsers_BuyerID",
                table: "Chats",
                column: "BuyerID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_AspNetUsers_SellerID",
                table: "Chats",
                column: "SellerID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_AspNetUsers_UserID",
                table: "Items",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Review_ReviewID",
                table: "Items",
                column: "ReviewID",
                principalTable: "Review",
                principalColumn: "ID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_AspNetUsers_UserId",
                table: "Payment",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_AspNetUsers_UserID",
                table: "Review",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.DropColumn(
    name: "Status",
    table: "Items");

            migrationBuilder.AddColumn<bool>(
                name: "IsReviewed",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
