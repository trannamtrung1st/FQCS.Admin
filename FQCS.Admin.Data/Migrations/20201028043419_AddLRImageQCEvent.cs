using Microsoft.EntityFrameworkCore.Migrations;

namespace FQCS.Admin.Data.Migrations
{
    public partial class AddLRImageQCEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LeftImage",
                table: "QCEvent",
                unicode: false,
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RightImage",
                table: "QCEvent",
                unicode: false,
                maxLength: 2000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeftImage",
                table: "QCEvent");

            migrationBuilder.DropColumn(
                name: "RightImage",
                table: "QCEvent");
        }
    }
}
