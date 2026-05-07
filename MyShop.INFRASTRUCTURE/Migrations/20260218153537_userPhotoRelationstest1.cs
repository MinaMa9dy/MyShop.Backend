using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyShop.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class userPhotoRelationstest1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserPhotos_UserPhotoId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserPhotoId",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_UserPhotos_UserId",
                table: "UserPhotos",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPhotos_AspNetUsers_UserId",
                table: "UserPhotos",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPhotos_AspNetUsers_UserId",
                table: "UserPhotos");

            migrationBuilder.DropIndex(
                name: "IX_UserPhotos_UserId",
                table: "UserPhotos");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserPhotoId",
                table: "AspNetUsers",
                column: "UserPhotoId",
                unique: true,
                filter: "[UserPhotoId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserPhotos_UserPhotoId",
                table: "AspNetUsers",
                column: "UserPhotoId",
                principalTable: "UserPhotos",
                principalColumn: "Id");
        }
    }
}
