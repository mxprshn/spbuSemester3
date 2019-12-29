using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyNUnitWeb.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssemblyModels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssemblyModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestModels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    ClassName = table.Column<string>(nullable: true),
                    IsPassed = table.Column<bool>(nullable: true),
                    IsIgnored = table.Column<bool>(nullable: false),
                    IgnoreReason = table.Column<string>(nullable: true),
                    RunTime = table.Column<TimeSpan>(nullable: false),
                    AssemblyModelId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestModels_AssemblyModels_AssemblyModelId",
                        column: x => x.AssemblyModelId,
                        principalTable: "AssemblyModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestModels_AssemblyModelId",
                table: "TestModels",
                column: "AssemblyModelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestModels");

            migrationBuilder.DropTable(
                name: "AssemblyModels");
        }
    }
}
