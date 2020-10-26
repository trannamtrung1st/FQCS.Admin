using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FQCS.Admin.Data.Migrations
{
    public partial class AddEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Disabled",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AppEvent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Data = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUser_AppEvent",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DefectType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    SampleImage = table.Column<string>(unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefectType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionLine",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    Info = table.Column<string>(maxLength: 2000, nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Disabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionLine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductModel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    Info = table.Column<string>(maxLength: 2000, nullable: true),
                    Image = table.Column<string>(unicode: false, nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QCDevice",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    ProductionLineId = table.Column<int>(nullable: false),
                    Archived = table.Column<bool>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QCDevice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionLine_QCDevice",
                        column: x => x.ProductionLineId,
                        principalTable: "ProductionLine",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductionBatch",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionLineId = table.Column<int>(nullable: false),
                    ProductModelId = table.Column<int>(nullable: false),
                    TotalAmount = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    StartedTime = table.Column<DateTime>(nullable: true),
                    FinishedTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionBatch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductModel_ProductionBatch",
                        column: x => x.ProductModelId,
                        principalTable: "ProductModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductionLine_ProductionBatch",
                        column: x => x.ProductionLineId,
                        principalTable: "ProductionLine",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QCEvent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    DefectTypeId = table.Column<int>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    ProductionBatchId = table.Column<int>(nullable: false),
                    QCDeviceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QCEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefectType_QCEvent",
                        column: x => x.DefectTypeId,
                        principalTable: "DefectType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductionBatch_QCEvent",
                        column: x => x.ProductionBatchId,
                        principalTable: "ProductionBatch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QCDevice_QCEvent",
                        column: x => x.QCDeviceId,
                        principalTable: "QCDevice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppEvent_UserId",
                table: "AppEvent",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionBatch_ProductModelId",
                table: "ProductionBatch",
                column: "ProductModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionBatch_ProductionLineId",
                table: "ProductionBatch",
                column: "ProductionLineId");

            migrationBuilder.CreateIndex(
                name: "IX_QCDevice_ProductionLineId",
                table: "QCDevice",
                column: "ProductionLineId");

            migrationBuilder.CreateIndex(
                name: "IX_QCEvent_DefectTypeId",
                table: "QCEvent",
                column: "DefectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_QCEvent_ProductionBatchId",
                table: "QCEvent",
                column: "ProductionBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_QCEvent_QCDeviceId",
                table: "QCEvent",
                column: "QCDeviceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppEvent");

            migrationBuilder.DropTable(
                name: "QCEvent");

            migrationBuilder.DropTable(
                name: "DefectType");

            migrationBuilder.DropTable(
                name: "ProductionBatch");

            migrationBuilder.DropTable(
                name: "QCDevice");

            migrationBuilder.DropTable(
                name: "ProductModel");

            migrationBuilder.DropTable(
                name: "ProductionLine");

            migrationBuilder.DropColumn(
                name: "Disabled",
                table: "AspNetUsers");
        }
    }
}
