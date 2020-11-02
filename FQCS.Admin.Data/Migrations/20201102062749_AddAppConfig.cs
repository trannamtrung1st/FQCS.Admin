using Microsoft.EntityFrameworkCore.Migrations;

namespace FQCS.Admin.Data.Migrations
{
    public partial class AddAppConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "50d97b41-327e-4398-b999-bd829f919894");

            migrationBuilder.CreateTable(
                name: "AppConfig",
                columns: table => new
                {
                    Id = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    ClientId = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    ClientSecret = table.Column<string>(unicode: false, maxLength: 500, nullable: true),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppConfig", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "5aa5012b-1ed5-4268-86c1-2777692f0d89", "a382e7e5-5b11-47c1-8ad4-ac570488c095", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppConfig");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5aa5012b-1ed5-4268-86c1-2777692f0d89");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "50d97b41-327e-4398-b999-bd829f919894", "3132ee4a-e597-49c4-af83-7866cecfe42c", "Administrator", "ADMINISTRATOR" });
        }
    }
}
