using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyShop.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class MovingPopularity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Popularity",
                table: "ProductVariants");

            migrationBuilder.AddColumn<int>(
                name: "Popularity",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Popularity",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "Popularity",
                table: "ProductVariants",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
