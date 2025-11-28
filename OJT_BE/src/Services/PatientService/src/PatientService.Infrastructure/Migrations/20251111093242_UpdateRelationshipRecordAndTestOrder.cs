using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationshipRecordAndTestOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestOrderId",
                schema: "patient",
                table: "PatientMedicalRecords");

            migrationBuilder.AddColumn<Guid>(
                name: "RecordId",
                schema: "patient",
                table: "TestOrder",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecordId",
                schema: "patient",
                table: "TestOrder");

            migrationBuilder.AddColumn<Guid>(
                name: "TestOrderId",
                schema: "patient",
                table: "PatientMedicalRecords",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
