using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyShop.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class ProductPhotoEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Path",
                table: "ProductPhotos",
                newName: "RelativePath");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "ProductPhotos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ProductPhotos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "ProductPhotos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "ProductPhotos",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "ProductPhotos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "ProductPhotos");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ProductPhotos");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "ProductPhotos");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "ProductPhotos");

            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "ProductPhotos");

            migrationBuilder.RenameColumn(
                name: "RelativePath",
                table: "ProductPhotos",
                newName: "Path");
        }
    }
}
