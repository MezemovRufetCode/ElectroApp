using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectroApp.Migrations
{
    public partial class productIsSpecial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSpecial",
                table: "Products",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSpecial",
                table: "Products");
        }
    }
}
