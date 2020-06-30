using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplicationAPI15_SecondStageTS_.Migrations
{
    public partial class IChangedUserModel6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnnouncementsCount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AnnouncementsCount2",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnnouncementsCount",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AnnouncementsCount2",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
