using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentSemesterResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "STUDENT_SEMESTER_RESULT",
                columns: table => new
                {
                    StudentId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    ClassId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SchoolYearId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    SemesterId = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    ConductId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    AcademicPerformanceId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    AverageSemester = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STUDENT_SEMESTER_RESULT", x => new { x.StudentId, x.ClassId, x.SchoolYearId, x.SemesterId });
                    table.ForeignKey(
                        name: "FK_STUDENT_SEMESTER_RESULT_ACADEMIC_PERFORMANCE_AcademicPerfor~",
                        column: x => x.AcademicPerformanceId,
                        principalTable: "ACADEMIC_PERFORMANCE",
                        principalColumn: "AcademicPerformanceId");
                    table.ForeignKey(
                        name: "FK_STUDENT_SEMESTER_RESULT_CLASS_ClassId",
                        column: x => x.ClassId,
                        principalTable: "CLASS",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STUDENT_SEMESTER_RESULT_CONDUCT_ConductId",
                        column: x => x.ConductId,
                        principalTable: "CONDUCT",
                        principalColumn: "ConductId");
                    table.ForeignKey(
                        name: "FK_STUDENT_SEMESTER_RESULT_SCHOOL_YEAR_SchoolYearId",
                        column: x => x.SchoolYearId,
                        principalTable: "SCHOOL_YEAR",
                        principalColumn: "SchoolYearId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STUDENT_SEMESTER_RESULT_SEMESTER_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "SEMESTER",
                        principalColumn: "SemesterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STUDENT_SEMESTER_RESULT_STUDENT_StudentId",
                        column: x => x.StudentId,
                        principalTable: "STUDENT",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_SEMESTER_RESULT_AcademicPerformanceId",
                table: "STUDENT_SEMESTER_RESULT",
                column: "AcademicPerformanceId");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_SEMESTER_RESULT_ClassId",
                table: "STUDENT_SEMESTER_RESULT",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_SEMESTER_RESULT_ConductId",
                table: "STUDENT_SEMESTER_RESULT",
                column: "ConductId");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_SEMESTER_RESULT_SchoolYearId",
                table: "STUDENT_SEMESTER_RESULT",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_SEMESTER_RESULT_SemesterId",
                table: "STUDENT_SEMESTER_RESULT",
                column: "SemesterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "STUDENT_SEMESTER_RESULT");
        }
    }
}
