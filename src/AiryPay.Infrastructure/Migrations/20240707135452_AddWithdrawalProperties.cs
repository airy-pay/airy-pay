using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiryPay.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWithdrawalProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_shops_ShopId",
                table: "products");

            migrationBuilder.AddColumn<string>(
                name: "ReceivingAccountNumber",
                table: "withdrawals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<long>(
                name: "ShopId",
                table: "products",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_products_shops_ShopId",
                table: "products",
                column: "ShopId",
                principalTable: "shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_shops_ShopId",
                table: "products");

            migrationBuilder.DropColumn(
                name: "ReceivingAccountNumber",
                table: "withdrawals");

            migrationBuilder.AlterColumn<long>(
                name: "ShopId",
                table: "products",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_products_shops_ShopId",
                table: "products",
                column: "ShopId",
                principalTable: "shops",
                principalColumn: "Id");
        }
    }
}
