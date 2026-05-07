using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyShop.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDuplicateCouponUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCoupons_Coupons_CouponCode",
                table: "UserCoupons");

            migrationBuilder.DropTable(
                name: "CouponUser");

            migrationBuilder.RenameColumn(
                name: "CouponCode",
                table: "UserCoupons",
                newName: "CouponId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCoupons_CouponCode",
                table: "UserCoupons",
                newName: "IX_UserCoupons_CouponId");

            migrationBuilder.AlterColumn<string>(
                name: "CouponCode",
                table: "Coupons",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCoupons_Coupons_CouponId",
                table: "UserCoupons",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCoupons_Coupons_CouponId",
                table: "UserCoupons");

            migrationBuilder.RenameColumn(
                name: "CouponId",
                table: "UserCoupons",
                newName: "CouponCode");

            migrationBuilder.RenameIndex(
                name: "IX_UserCoupons_CouponId",
                table: "UserCoupons",
                newName: "IX_UserCoupons_CouponCode");

            migrationBuilder.AlterColumn<Guid>(
                name: "CouponCode",
                table: "Coupons",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "CouponUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CouponId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouponUser_Coupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "Coupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CouponUser_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CouponUser_CouponId",
                table: "CouponUser",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponUser_CustomerId",
                table: "CouponUser",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCoupons_Coupons_CouponCode",
                table: "UserCoupons",
                column: "CouponCode",
                principalTable: "Coupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
