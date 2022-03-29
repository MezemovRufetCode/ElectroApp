using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectroApp.Migrations
{
    public partial class SliderCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntroSliders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BackImage = table.Column<string>(nullable: true),
                    BackImageMobile = table.Column<string>(nullable: true),
                    FirstDesc = table.Column<string>(nullable: true),
                    SecDesc = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    SliderBannerImage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntroSliders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntroSliders");
        }
    }
}
