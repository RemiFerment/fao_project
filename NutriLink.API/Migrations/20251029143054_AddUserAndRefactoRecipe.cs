using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriLink.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAndRefactoRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Calories",
                table: "Recipes");

            migrationBuilder.AddColumn<string>(
                name: "steps",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "steps",
                table: "Recipes");

            migrationBuilder.AddColumn<int>(
                name: "Calories",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
