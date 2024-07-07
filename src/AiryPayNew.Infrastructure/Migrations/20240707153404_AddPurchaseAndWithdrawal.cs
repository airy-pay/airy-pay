using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiryPayNew.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseAndWithdrawal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bills_products_ProductId",
                table: "bills");

            migrationBuilder.DropForeignKey(
                name: "FK_purchases_bills_BillId",
                table: "purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_purchases_shops_ShopId",
                table: "purchases");

            migrationBuilder.DropIndex(
                name: "IX_purchases_BillId",
                table: "purchases");

            migrationBuilder.AlterColumn<long>(
                name: "ShopId",
                table: "purchases",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BillId",
                table: "purchases",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "purchases",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscordRoleId",
                table: "products",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ProductId",
                table: "bills",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BuyerDiscordId",
                table: "bills",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_purchases_BillId",
                table: "purchases",
                column: "BillId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_purchases_ProductId",
                table: "purchases",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_bills_products_ProductId",
                table: "bills",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_bills_BillId",
                table: "purchases",
                column: "BillId",
                principalTable: "bills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_products_ProductId",
                table: "purchases",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_shops_ShopId",
                table: "purchases",
                column: "ShopId",
                principalTable: "shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bills_products_ProductId",
                table: "bills");

            migrationBuilder.DropForeignKey(
                name: "FK_purchases_bills_BillId",
                table: "purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_purchases_products_ProductId",
                table: "purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_purchases_shops_ShopId",
                table: "purchases");

            migrationBuilder.DropIndex(
                name: "IX_purchases_BillId",
                table: "purchases");

            migrationBuilder.DropIndex(
                name: "IX_purchases_ProductId",
                table: "purchases");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "purchases");

            migrationBuilder.DropColumn(
                name: "DiscordRoleId",
                table: "products");

            migrationBuilder.DropColumn(
                name: "BuyerDiscordId",
                table: "bills");

            migrationBuilder.AlterColumn<long>(
                name: "ShopId",
                table: "purchases",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "BillId",
                table: "purchases",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "ProductId",
                table: "bills",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_purchases_BillId",
                table: "purchases",
                column: "BillId");

            migrationBuilder.AddForeignKey(
                name: "FK_bills_products_ProductId",
                table: "bills",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_bills_BillId",
                table: "purchases",
                column: "BillId",
                principalTable: "bills",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_shops_ShopId",
                table: "purchases",
                column: "ShopId",
                principalTable: "shops",
                principalColumn: "Id");
        }
    }
}
