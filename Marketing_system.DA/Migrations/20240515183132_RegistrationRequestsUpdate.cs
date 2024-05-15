using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketing_system.DA.Migrations
{
    /// <inheritdoc />
    public partial class RegistrationRequestsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountStatus",
                table: "RegistrationRequests");

            migrationBuilder.DropColumn(
                name: "Firstname",
                table: "RegistrationRequests");

            migrationBuilder.DropColumn(
                name: "Lastname",
                table: "RegistrationRequests");

            migrationBuilder.RenameColumn(
                name: "RegistrationDateTime",
                table: "RegistrationRequests",
                newName: "RegistrationDate");

            migrationBuilder.RenameColumn(
                name: "PackageType",
                table: "RegistrationRequests",
                newName: "Status");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "RegistrationRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "RegistrationRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenExpirationDate",
                table: "RegistrationRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "RegistrationRequests",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "RegistrationRequests");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "RegistrationRequests");

            migrationBuilder.DropColumn(
                name: "TokenExpirationDate",
                table: "RegistrationRequests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RegistrationRequests");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "RegistrationRequests",
                newName: "PackageType");

            migrationBuilder.RenameColumn(
                name: "RegistrationDate",
                table: "RegistrationRequests",
                newName: "RegistrationDateTime");

            migrationBuilder.AddColumn<int>(
                name: "AccountStatus",
                table: "RegistrationRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Firstname",
                table: "RegistrationRequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Lastname",
                table: "RegistrationRequests",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
