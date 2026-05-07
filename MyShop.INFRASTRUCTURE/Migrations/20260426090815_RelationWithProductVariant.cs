using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyShop.INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class RelationWithProductVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCoupons_Coupons_CouponCode",
                table: "ProductCoupons");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCoupons_Products_ProductId",
                table: "ProductCoupons");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCoupons_AspNetUsers_UserId",
                table: "UserCoupons");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "ProductPhotos");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "ProductPhotos");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserCoupons",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCoupons_UserId",
                table: "UserCoupons",
                newName: "IX_UserCoupons_CustomerId");

            migrationBuilder.RenameColumn(
                name: "CouponCode",
                table: "ProductCoupons",
                newName: "CouponId");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "ProductCoupons",
                newName: "ProductVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCoupons_CouponCode",
                table: "ProductCoupons",
                newName: "IX_ProductCoupons_CouponId");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "CartItems",
                newName: "ProductVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                newName: "IX_CartItems_ProductVariantId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProductCoupons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_ProductVariants_ProductVariantId",
                table: "CartItems",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCoupons_Coupons_CouponId",
                table: "ProductCoupons",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCoupons_ProductVariants_ProductVariantId",
                table: "ProductCoupons",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCoupons_Customers_CustomerId",
                table: "UserCoupons",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_ProductVariants_ProductVariantId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCoupons_Coupons_CouponId",
                table: "ProductCoupons");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCoupons_ProductVariants_ProductVariantId",
                table: "ProductCoupons");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCoupons_Customers_CustomerId",
                table: "UserCoupons");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductCoupons");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "UserCoupons",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCoupons_CustomerId",
                table: "UserCoupons",
                newName: "IX_UserCoupons_UserId");

            migrationBuilder.RenameColumn(
                name: "CouponId",
                table: "ProductCoupons",
                newName: "CouponCode");

            migrationBuilder.RenameColumn(
                name: "ProductVariantId",
                table: "ProductCoupons",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCoupons_CouponId",
                table: "ProductCoupons",
                newName: "IX_ProductCoupons_CouponCode");

            migrationBuilder.RenameColumn(
                name: "ProductVariantId",
                table: "CartItems",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_ProductVariantId",
                table: "CartItems",
                newName: "IX_CartItems_ProductId");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
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

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCoupons_Coupons_CouponCode",
                table: "ProductCoupons",
                column: "CouponCode",
                principalTable: "Coupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCoupons_Products_ProductId",
                table: "ProductCoupons",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCoupons_AspNetUsers_UserId",
                table: "UserCoupons",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
