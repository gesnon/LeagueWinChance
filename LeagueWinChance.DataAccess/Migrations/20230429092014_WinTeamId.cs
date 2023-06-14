using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeagueWinChance.DataAccess.Migrations
{
    public partial class WinTeamId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Win",
                table: "Matches");

            migrationBuilder.AddColumn<int>(
                name: "TeamWinId",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamWinId",
                table: "Matches");

            migrationBuilder.AddColumn<bool>(
                name: "Win",
                table: "Matches",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
