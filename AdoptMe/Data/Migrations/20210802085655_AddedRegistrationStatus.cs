using Microsoft.EntityFrameworkCore.Migrations;

namespace AdoptMe.Data.Migrations
{
    public partial class AddedRegistrationStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegistrationStatus",
                table: "Shelters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationStatus",
                table: "Shelters");
        }
    }
}
