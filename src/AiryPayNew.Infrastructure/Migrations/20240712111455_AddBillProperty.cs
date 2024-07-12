using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiryPayNew.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBillProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bills_shops_ShopId",
                table: "bills");

            migrationBuilder.DropForeignKey(
                name: "FK_withdrawals_shops_ShopId",
                table: "withdrawals");

            migrationBuilder.AlterColumn<long>(
                name: "ShopId",
                table: "withdrawals",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ShopId",
                table: "bills",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodId",
                table: "bills",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "PaymentSystemId",
                table: "bills",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "PaymentSystemName",
                table: "bills",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_bills_shops_ShopId",
                table: "bills",
                column: "ShopId",
                principalTable: "shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_withdrawals_shops_ShopId",
                table: "withdrawals",
                column: "ShopId",
                principalTable: "shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bills_shops_ShopId",
                table: "bills");

            migrationBuilder.DropForeignKey(
                name: "FK_withdrawals_shops_ShopId",
                table: "withdrawals");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "bills");

            migrationBuilder.DropColumn(
                name: "PaymentSystemId",
                table: "bills");

            migrationBuilder.DropColumn(
                name: "PaymentSystemName",
                table: "bills");

            migrationBuilder.AlterColumn<long>(
                name: "ShopId",
                table: "withdrawals",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ShopId",
                table: "bills",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_bills_shops_ShopId",
                table: "bills",
                column: "ShopId",
                principalTable: "shops",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_withdrawals_shops_ShopId",
                table: "withdrawals",
                column: "ShopId",
                principalTable: "shops",
                principalColumn: "Id");
        }
    }
}
