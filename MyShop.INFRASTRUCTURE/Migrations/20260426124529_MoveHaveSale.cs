using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyShop.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class MoveHaveSale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HaveSale",
                table: "ProductVariants");

            migrationBuilder.AddColumn<bool>(
                name: "HaveSale",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HaveSale",
                table: "Products");

            migrationBuilder.AddColumn<bool>(
                name: "HaveSale",
                table: "ProductVariants",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
