using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintsToPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IdentifyNumber",
                schema: "patient",
                table: "Patients",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "patient",
                table: "Patients",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Email",
                schema: "patient",
                table: "Patients",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_IdentifyNumber",
                schema: "patient",
                table: "Patients",
                column: "IdentifyNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PhoneNumber",
                schema: "patient",
                table: "Patients",
                column: "PhoneNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patients_Email",
                schema: "patient",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_IdentifyNumber",
                schema: "patient",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_PhoneNumber",
                schema: "patient",
                table: "Patients");

            migrationBuilder.AlterColumn<string>(
                name: "IdentifyNumber",
                schema: "patient",
                table: "Patients",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "patient",
                table: "Patients",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
