using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAMService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameRefreshTokenToJwtToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Rename table RefreshToken to JwtToken (GIỮ NGUYÊN DATA)
            migrationBuilder.RenameTable(
                name: "RefreshToken",
                newName: "JwtToken");

            // 2. Rename existing index
            migrationBuilder.RenameIndex(
                name: "IX_RefreshToken_UserId",
                table: "JwtToken",
                newName: "IX_JwtToken_UserId");

            // 3. Update Token column max length from 200 to 500
            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "JwtToken",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            // 4. Add new column TokenType with default value
            migrationBuilder.AddColumn<string>(
                name: "TokenType",
                table: "JwtToken",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "RefreshToken",
                comment: "Token type: RefreshToken or AccessToken");

            // 5. Update existing data to set TokenType for all existing rows
            migrationBuilder.Sql(@"
                UPDATE ""JwtToken"" 
                SET ""TokenType"" = 'RefreshToken' 
                WHERE ""TokenType"" IS NULL OR ""TokenType"" = '';
            ");

            // 6. Update ExpiresAt column type from 'timestamp with time zone' to 'timestamp without time zone'
            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiresAt",
                table: "JwtToken",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            // 7. Add default value for IsRevoked if not already set
            migrationBuilder.AlterColumn<bool>(
                name: "IsRevoked",
                table: "JwtToken",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            // 8. Create unique index on Token column
            migrationBuilder.CreateIndex(
                name: "IX_JwtToken_Token",
                table: "JwtToken",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Drop the unique index on Token
            migrationBuilder.DropIndex(
                name: "IX_JwtToken_Token",
                table: "JwtToken");

            // 2. Remove default value from IsRevoked
            migrationBuilder.AlterColumn<bool>(
                name: "IsRevoked",
                table: "JwtToken",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            // 3. Revert ExpiresAt column type
            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiresAt",
                table: "JwtToken",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            // 4. Remove TokenType column
            migrationBuilder.DropColumn(
                name: "TokenType",
                table: "JwtToken");

            // 5. Revert Token column max length
            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "JwtToken",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            // 6. Rename index back
            migrationBuilder.RenameIndex(
                name: "IX_JwtToken_UserId",
                table: "JwtToken",
                newName: "IX_RefreshToken_UserId");

            // 7. Rename table back
            migrationBuilder.RenameTable(
                name: "JwtToken",
                newName: "RefreshToken");
        }
    }
}
