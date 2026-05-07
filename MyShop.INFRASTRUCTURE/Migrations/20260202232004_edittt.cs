using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyShop.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class edittt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReviewCount",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewCount",
                table: "Products");
        }
    }
}
