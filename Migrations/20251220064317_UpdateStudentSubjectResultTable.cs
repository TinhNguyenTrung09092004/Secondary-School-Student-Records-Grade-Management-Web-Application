using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudentSubjectResultTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Average15Min",
                table: "STUDENT_SUBJECT_RESULT");

            migrationBuilder.DropColumn(
                name: "Average45Min",
                table: "STUDENT_SUBJECT_RESULT");

            migrationBuilder.DropColumn(
                name: "AverageOral",
                table: "STUDENT_SUBJECT_RESULT");

            migrationBuilder.DropColumn(
                name: "ExamScore",
                table: "STUDENT_SUBJECT_RESULT");

            migrationBuilder.AlterColumn<decimal>(
                name: "AverageSemester",
                table: "STUDENT_SUBJECT_RESULT",
                type: "numeric(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)");

            migrationBuilder.AddColumn<string>(
                name: "CommentResult",
                table: "STUDENT_SUBJECT_RESULT",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentResult",
                table: "STUDENT_SUBJECT_RESULT");

            migrationBuilder.AlterColumn<decimal>(
                name: "AverageSemester",
                table: "STUDENT_SUBJECT_RESULT",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Average15Min",
                table: "STUDENT_SUBJECT_RESULT",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Average45Min",
                table: "STUDENT_SUBJECT_RESULT",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AverageOral",
                table: "STUDENT_SUBJECT_RESULT",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ExamScore",
                table: "STUDENT_SUBJECT_RESULT",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
