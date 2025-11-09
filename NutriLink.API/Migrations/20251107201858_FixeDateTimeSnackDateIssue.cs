using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriLink.API.Migrations
{
    /// <inheritdoc />
    public partial class FixeDateTimeSnackDateIssue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "SnackDays",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "SnackDays");
        }
    }
}
