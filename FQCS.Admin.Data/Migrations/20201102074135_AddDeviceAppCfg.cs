using Microsoft.EntityFrameworkCore.Migrations;

namespace FQCS.Admin.Data.Migrations
{
    public partial class AddDeviceAppCfg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5aa5012b-1ed5-4268-86c1-2777692f0d89");

            migrationBuilder.AddColumn<string>(
                name: "AppConfigId",
                table: "QCDevice",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "2d524231-ab94-48b4-97fd-3e3205828aa2", "275fb8d4-e193-4268-9e93-df41cf1f8c4b", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.CreateIndex(
                name: "IX_QCDevice_AppConfigId",
                table: "QCDevice",
                column: "AppConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppConfig_QCDevice",
                table: "QCDevice",
                column: "AppConfigId",
                principalTable: "AppConfig",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppConfig_QCDevice",
                table: "QCDevice");

            migrationBuilder.DropIndex(
                name: "IX_QCDevice_AppConfigId",
                table: "QCDevice");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2d524231-ab94-48b4-97fd-3e3205828aa2");

            migrationBuilder.DropColumn(
                name: "AppConfigId",
                table: "QCDevice");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "5aa5012b-1ed5-4268-86c1-2777692f0d89", "a382e7e5-5b11-47c1-8ad4-ac570488c095", "Administrator", "ADMINISTRATOR" });
        }
    }
}
