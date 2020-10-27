using Microsoft.EntityFrameworkCore.Migrations;

namespace FQCS.Admin.Data.Migrations
{
    public partial class SetIsRequiredFalseQCDevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ProductionLineId",
                table: "QCDevice",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ProductionLineId",
                table: "QCDevice",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
