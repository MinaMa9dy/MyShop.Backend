using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyShop.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class EditSale_SuperCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HaveSale",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFasting",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Popularity",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SuperCategoryId",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_SuperCategoryId",
                table: "Categories",
                column: "SuperCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_SuperCategoryId",
                table: "Categories",
                column: "SuperCategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_SuperCategoryId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_SuperCategoryId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "HaveSale",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsFasting",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Popularity",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SuperCategoryId",
                table: "Categories");
        }
    }
}
