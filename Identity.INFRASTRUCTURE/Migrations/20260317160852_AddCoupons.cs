using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyShop.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class AddCoupons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    CouponCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CouponName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CouponDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.CouponCode);
                });

            migrationBuilder.CreateTable(
                name: "ProductCoupons",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CouponCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCoupons", x => new { x.ProductId, x.CouponCode });
                    table.ForeignKey(
                        name: "FK_ProductCoupons_Coupons_CouponCode",
                        column: x => x.CouponCode,
                        principalTable: "Coupons",
                        principalColumn: "CouponCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductCoupons_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCoupons_CouponCode",
                table: "ProductCoupons",
                column: "CouponCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductCoupons");

            migrationBuilder.DropTable(
                name: "Coupons");
        }
    }
}
