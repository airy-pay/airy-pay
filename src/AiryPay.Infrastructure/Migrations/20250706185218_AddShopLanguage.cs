using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiryPay.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShopLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "shops",
                type: "character varying(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "en");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "shops");
        }
    }
}
