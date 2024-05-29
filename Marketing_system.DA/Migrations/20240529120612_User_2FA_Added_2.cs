using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketing_system.DA.Migrations
{
    /// <inheritdoc />
    public partial class User_2FA_Added_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTwoFactorReady",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTwoFactorReady",
                table: "Users");
        }
    }
}
