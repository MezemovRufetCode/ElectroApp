using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectroApp.Migrations
{
    public partial class ordermessageadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "Orders");
        }
    }
}
