using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTestResultDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Flag",
                schema: "patient",
                table: "TestResult");

            migrationBuilder.DropColumn(
                name: "ReferenceRange",
                schema: "patient",
                table: "TestResult");

            migrationBuilder.DropColumn(
                name: "Value",
                schema: "patient",
                table: "TestResult");

            migrationBuilder.CreateTable(
                name: "TestResultDetail",
                schema: "patient",
                columns: table => new
                {
                    TestResultDetailId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResultId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    Flag = table.Column<string>(type: "text", nullable: false, defaultValue: "Normal"),
                    ReferenceRange = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResultDetail", x => x.TestResultDetailId);
                    table.ForeignKey(
                        name: "FK_TestResultDetail_TestResult_ResultId",
                        column: x => x.ResultId,
                        principalSchema: "patient",
                        principalTable: "TestResult",
                        principalColumn: "ResultId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestResultDetail_ResultId",
                schema: "patient",
                table: "TestResultDetail",
                column: "ResultId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestResultDetail",
                schema: "patient");

            migrationBuilder.AddColumn<string>(
                name: "Flag",
                schema: "patient",
                table: "TestResult",
                type: "text",
                nullable: false,
                defaultValue: "Normal");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceRange",
                schema: "patient",
                table: "TestResult",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Value",
                schema: "patient",
                table: "TestResult",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
