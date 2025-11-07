using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriLink.API.Migrations
{
    /// <inheritdoc />
    public partial class Finalize_Migrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetWeight",
                table: "AchievementType",
                newName: "Weight");

            migrationBuilder.AlterColumn<int>(
                name: "AchievementTypeId",
                table: "Achievements",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Weight",
                table: "AchievementType",
                newName: "TargetWeight");

            migrationBuilder.AlterColumn<int>(
                name: "AchievementTypeId",
                table: "Achievements",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
