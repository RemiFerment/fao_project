using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriLink.API.Migrations
{
    /// <inheritdoc />
    public partial class FixedCascadeDeletionForModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UsersProfile_UserProfileId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersProfile",
                table: "UsersProfile");

            migrationBuilder.RenameTable(
                name: "UsersProfile",
                newName: "UserProfiles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProfiles",
                table: "UserProfiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserProfiles_UserProfileId",
                table: "Users",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserProfiles_UserProfileId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProfiles",
                table: "UserProfiles");

            migrationBuilder.RenameTable(
                name: "UserProfiles",
                newName: "UsersProfile");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersProfile",
                table: "UsersProfile",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UsersProfile_UserProfileId",
                table: "Users",
                column: "UserProfileId",
                principalTable: "UsersProfile",
                principalColumn: "Id");
        }
    }
}
