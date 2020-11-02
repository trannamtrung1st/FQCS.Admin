using Microsoft.EntityFrameworkCore.Migrations;

namespace FQCS.Admin.Data.Migrations
{
    public partial class AddDeviceAPIBaseURL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a455ea2a-0362-4cf0-9b3e-75791f9c6045");

            migrationBuilder.AddColumn<string>(
                name: "DeviceAPIBaseUrl",
                table: "QCDevice",
                maxLength: 255,
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "de61be4a-b1f5-414e-ae8b-66b9a9779027", "2d80ad9d-5724-47de-8811-d59384ff62bc", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "de61be4a-b1f5-414e-ae8b-66b9a9779027");

            migrationBuilder.DropColumn(
                name: "DeviceAPIBaseUrl",
                table: "QCDevice");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "a455ea2a-0362-4cf0-9b3e-75791f9c6045", "6cd6d654-30ee-405d-b8a1-c98c0636eb6d", "Administrator", "ADMINISTRATOR" });
        }
    }
}
