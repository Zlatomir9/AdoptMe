using Microsoft.EntityFrameworkCore.Migrations;

namespace AdoptMe.Data.Migrations
{
    public partial class SheltersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShelterId",
                table: "Pets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Shelters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shelters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shelters_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pets_ShelterId",
                table: "Pets",
                column: "ShelterId");

            migrationBuilder.CreateIndex(
                name: "IX_Shelters_UserId",
                table: "Shelters",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_Shelters_ShelterId",
                table: "Pets",
                column: "ShelterId",
                principalTable: "Shelters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Shelters_ShelterId",
                table: "Pets");

            migrationBuilder.DropTable(
                name: "Shelters");

            migrationBuilder.DropIndex(
                name: "IX_Pets_ShelterId",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "ShelterId",
                table: "Pets");
        }
    }
}
