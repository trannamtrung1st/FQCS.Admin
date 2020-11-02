using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FQCS.Admin.Data.Migrations
{
    public partial class AddDtAppCfg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2d524231-ab94-48b4-97fd-3e3205828aa2");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "AppConfig",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "AppConfig",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "a455ea2a-0362-4cf0-9b3e-75791f9c6045", "6cd6d654-30ee-405d-b8a1-c98c0636eb6d", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a455ea2a-0362-4cf0-9b3e-75791f9c6045");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "AppConfig");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "AppConfig");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "2d524231-ab94-48b4-97fd-3e3205828aa2", "275fb8d4-e193-4268-9e93-df41cf1f8c4b", "Administrator", "ADMINISTRATOR" });
        }
    }
}
