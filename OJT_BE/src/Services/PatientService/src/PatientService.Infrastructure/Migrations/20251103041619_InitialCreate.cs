using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "patient");

            migrationBuilder.CreateTable(
                name: "Patients",
                schema: "patient",
                columns: table => new
                {
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Gender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastTestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IdentifyNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.PatientId);
                });

            migrationBuilder.CreateTable(
                name: "PatientMedicalRecords",
                schema: "patient",
                columns: table => new
                {
                    RecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicalNotes = table.Column<string>(type: "text", nullable: true),
                    Diagnosis = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientMedicalRecords", x => x.RecordId);
                    table.ForeignKey(
                        name: "FK_PatientMedicalRecords_Patients_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "patient",
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientRecordAuditLogs",
                schema: "patient",
                columns: table => new
                {
                    AuditId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PerformedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChangedFields = table.Column<string>(type: "text", nullable: true),
                    OldValues = table.Column<string>(type: "text", nullable: true),
                    NewValues = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientRecordAuditLogs", x => x.AuditId);
                    table.ForeignKey(
                        name: "FK_PatientRecordAuditLogs_PatientMedicalRecords_RecordId",
                        column: x => x.RecordId,
                        principalSchema: "patient",
                        principalTable: "PatientMedicalRecords",
                        principalColumn: "RecordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedicalRecords_PatientId",
                schema: "patient",
                table: "PatientMedicalRecords",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientRecordAuditLogs_RecordId",
                schema: "patient",
                table: "PatientRecordAuditLogs",
                column: "RecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientRecordAuditLogs",
                schema: "patient");

            migrationBuilder.DropTable(
                name: "PatientMedicalRecords",
                schema: "patient");

            migrationBuilder.DropTable(
                name: "Patients",
                schema: "patient");
        }
    }
}
