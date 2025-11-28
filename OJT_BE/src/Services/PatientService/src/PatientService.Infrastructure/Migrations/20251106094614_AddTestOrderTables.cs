using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PatientService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTestOrderTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClinicalNotes",
                schema: "patient",
                table: "PatientMedicalRecords");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "patient",
                table: "PatientMedicalRecords");

            migrationBuilder.DropColumn(
                name: "Diagnosis",
                schema: "patient",
                table: "PatientMedicalRecords");

            migrationBuilder.RenameColumn(
                name: "DoctorId",
                schema: "patient",
                table: "PatientMedicalRecords",
                newName: "TestOrderId");

            migrationBuilder.CreateTable(
                name: "FlaggingSetConfig",
                schema: "patient",
                columns: table => new
                {
                    ConfigId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestName = table.Column<string>(type: "text", nullable: false),
                    LowThreshold = table.Column<float>(type: "real", nullable: true),
                    HighThreshold = table.Column<float>(type: "real", nullable: true),
                    CriticalThreshold = table.Column<float>(type: "real", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false, defaultValue: "1.0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlaggingSetConfig", x => x.ConfigId);
                });

            migrationBuilder.CreateTable(
                name: "TestOrder",
                schema: "patient",
                columns: table => new
                {
                    TestOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientName = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Age = table.Column<int>(type: "integer", nullable: true),
                    Gender = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Pending"),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RunBy = table.Column<string>(type: "text", nullable: true),
                    RunOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedBy = table.Column<string>(type: "text", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestOrder", x => x.TestOrderId);
                });

            migrationBuilder.CreateTable(
                name: "TestOrderAuditLog",
                schema: "patient",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    ActionType = table.Column<string>(type: "text", nullable: false),
                    ChangedFields = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestOrderAuditLog", x => x.LogId);
                });

            migrationBuilder.CreateTable(
                name: "TestResult",
                schema: "patient",
                columns: table => new
                {
                    ResultId = table.Column<Guid>(type: "uuid", nullable: false),
                    TestOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    TestName = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    ReferenceRange = table.Column<string>(type: "text", nullable: true),
                    Interpretation = table.Column<string>(type: "text", nullable: true),
                    InstrumentUsed = table.Column<string>(type: "text", nullable: true),
                    Flag = table.Column<string>(type: "text", nullable: false, defaultValue: "Normal"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResult", x => x.ResultId);
                    table.ForeignKey(
                        name: "FK_TestResult_TestOrder_TestOrderId",
                        column: x => x.TestOrderId,
                        principalSchema: "patient",
                        principalTable: "TestOrder",
                        principalColumn: "TestOrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                schema: "patient",
                columns: table => new
                {
                    CommentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TestOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResultId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_Comment_TestOrder_TestOrderId",
                        column: x => x.TestOrderId,
                        principalSchema: "patient",
                        principalTable: "TestOrder",
                        principalColumn: "TestOrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comment_TestResult_ResultId",
                        column: x => x.ResultId,
                        principalSchema: "patient",
                        principalTable: "TestResult",
                        principalColumn: "ResultId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ResultId",
                schema: "patient",
                table: "Comment",
                column: "ResultId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_TestOrderId",
                schema: "patient",
                table: "Comment",
                column: "TestOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResult_TestOrderId",
                schema: "patient",
                table: "TestResult",
                column: "TestOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment",
                schema: "patient");

            migrationBuilder.DropTable(
                name: "FlaggingSetConfig",
                schema: "patient");

            migrationBuilder.DropTable(
                name: "TestOrderAuditLog",
                schema: "patient");

            migrationBuilder.DropTable(
                name: "TestResult",
                schema: "patient");

            migrationBuilder.DropTable(
                name: "TestOrder",
                schema: "patient");

            migrationBuilder.RenameColumn(
                name: "TestOrderId",
                schema: "patient",
                table: "PatientMedicalRecords",
                newName: "DoctorId");

            migrationBuilder.AddColumn<string>(
                name: "ClinicalNotes",
                schema: "patient",
                table: "PatientMedicalRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                schema: "patient",
                table: "PatientMedicalRecords",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Diagnosis",
                schema: "patient",
                table: "PatientMedicalRecords",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
