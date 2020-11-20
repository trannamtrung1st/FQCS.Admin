using Microsoft.EntityFrameworkCore.Migrations;

namespace FQCS.Admin.Data.Migrations
{
    public partial class AddSeenColQCEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "358109eb-d236-4410-86b6-0c9a88136a5c");

            migrationBuilder.AddColumn<bool>(
                name: "Seen",
                table: "QCEvent",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "5fbb769c-26e6-4110-9570-f34124c1939e", "72dce78e-c5e3-4381-b478-f4cdc85719bf", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5fbb769c-26e6-4110-9570-f34124c1939e");

            migrationBuilder.DropColumn(
                name: "Seen",
                table: "QCEvent");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "358109eb-d236-4410-86b6-0c9a88136a5c", "b3eb9030-8310-4fdf-986b-07813e4e468e", "Administrator", "ADMINISTRATOR" });
        }
    }
}
