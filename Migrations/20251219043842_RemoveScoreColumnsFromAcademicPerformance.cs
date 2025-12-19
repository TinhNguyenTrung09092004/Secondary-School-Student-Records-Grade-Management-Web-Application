using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveScoreColumnsFromAcademicPerformance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ControlScore",
                table: "ACADEMIC_PERFORMANCE");

            migrationBuilder.DropColumn(
                name: "MaxRequiredScore",
                table: "ACADEMIC_PERFORMANCE");

            migrationBuilder.DropColumn(
                name: "MinRequiredScore",
                table: "ACADEMIC_PERFORMANCE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ControlScore",
                table: "ACADEMIC_PERFORMANCE",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxRequiredScore",
                table: "ACADEMIC_PERFORMANCE",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinRequiredScore",
                table: "ACADEMIC_PERFORMANCE",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
