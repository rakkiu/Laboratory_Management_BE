using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTestOrderEntityV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "patient",
                table: "TestOrder",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                schema: "patient",
                table: "TestOrder");
        }
    }
}
