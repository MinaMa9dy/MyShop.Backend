using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyShop.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class changeName_Id_To_UserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_AspNetUsers_Id",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Sellers_AspNetUsers_Id",
                table: "Sellers");

            migrationBuilder.DropForeignKey(
                name: "FK_WishLists_Customers_CustomerId",
                table: "WishLists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WishLists",
                table: "WishLists");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "WishLists");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Sellers",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Customers",
                newName: "UserId");

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "WishLists",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WishLists",
                table: "WishLists",
                columns: new[] { "ProductId", "CustomerId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_AspNetUsers_UserId",
                table: "Customers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sellers_AspNetUsers_UserId",
                table: "Sellers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WishLists_Customers_CustomerId",
                table: "WishLists",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_AspNetUsers_UserId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Sellers_AspNetUsers_UserId",
                table: "Sellers");

            migrationBuilder.DropForeignKey(
                name: "FK_WishLists_Customers_CustomerId",
                table: "WishLists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WishLists",
                table: "WishLists");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Sellers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Customers",
                newName: "Id");

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "WishLists",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "WishLists",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_WishLists",
                table: "WishLists",
                columns: new[] { "ProductId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_AspNetUsers_Id",
                table: "Customers",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sellers_AspNetUsers_Id",
                table: "Sellers",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WishLists_Customers_CustomerId",
                table: "WishLists",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");
        }
    }
}
