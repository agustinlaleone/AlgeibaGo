using Microsoft.EntityFrameworkCore.Migrations;

namespace AlgeibaGoAPI.Migrations
{
    public partial class AddUsersColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PersonName",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "PersonSurname",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
               name: "PersonName",
               table: "AspNetUsers");
            migrationBuilder.DropColumn(
               name: "PersonSurname",
               table: "AspNetUsers");
        }
    }
}
