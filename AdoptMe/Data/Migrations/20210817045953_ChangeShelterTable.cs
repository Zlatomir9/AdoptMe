using Microsoft.EntityFrameworkCore.Migrations;

namespace AdoptMe.Data.Migrations
{
    public partial class ChangeShelterTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Shelters");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Shelters",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shelters_UserId1",
                table: "Shelters",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Shelters_AspNetUsers_UserId1",
                table: "Shelters",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shelters_AspNetUsers_UserId1",
                table: "Shelters");

            migrationBuilder.DropIndex(
                name: "IX_Shelters_UserId1",
                table: "Shelters");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Shelters");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Shelters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
