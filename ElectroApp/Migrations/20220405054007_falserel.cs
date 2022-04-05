using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectroApp.Migrations
{
    public partial class falserel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Specs_SpecsId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SpecsId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SpecsId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Specs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Specs_ProductId",
                table: "Specs",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Specs_Products_ProductId",
                table: "Specs",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Specs_Products_ProductId",
                table: "Specs");

            migrationBuilder.DropIndex(
                name: "IX_Specs_ProductId",
                table: "Specs");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Specs");

            migrationBuilder.AddColumn<int>(
                name: "SpecsId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SpecsId",
                table: "Products",
                column: "SpecsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Specs_SpecsId",
                table: "Products",
                column: "SpecsId",
                principalTable: "Specs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
