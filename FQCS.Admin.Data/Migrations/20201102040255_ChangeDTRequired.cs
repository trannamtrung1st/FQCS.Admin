using Microsoft.EntityFrameworkCore.Migrations;

namespace FQCS.Admin.Data.Migrations
{
    public partial class ChangeDTRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7f5bb645-b211-45fd-8068-1a048947e97d");

            migrationBuilder.AlterColumn<int>(
                name: "DefectTypeId",
                table: "QCEvent",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "50d97b41-327e-4398-b999-bd829f919894", "3132ee4a-e597-49c4-af83-7866cecfe42c", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "50d97b41-327e-4398-b999-bd829f919894");

            migrationBuilder.AlterColumn<int>(
                name: "DefectTypeId",
                table: "QCEvent",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7f5bb645-b211-45fd-8068-1a048947e97d", "8f3ed609-1863-45c3-bad8-527ff511c552", "Administrator", "ADMINISTRATOR" });
        }
    }
}
