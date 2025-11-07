using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "users");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "AspNetUserTokens",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "AspNetUsers",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "AspNetUserRoles",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "AspNetUserLogins",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "AspNetUserClaims",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "AspNetRoles",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "AspNetRoleClaims",
                newSchema: "users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "users",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                schema: "users",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                schema: "users",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "users",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "users",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                schema: "users",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "users",
                newName: "AspNetRoleClaims");
        }
    }
}
