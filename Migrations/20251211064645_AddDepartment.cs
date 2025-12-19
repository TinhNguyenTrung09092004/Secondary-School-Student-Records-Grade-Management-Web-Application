using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDepartmentHead",
                table: "TEACHER");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentId",
                table: "TEACHER",
                type: "character varying(6)",
                maxLength: 6,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DEPARTMENT",
                columns: table => new
                {
                    DepartmentId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    DepartmentName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    HeadTeacherId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DEPARTMENT", x => x.DepartmentId);
                    table.ForeignKey(
                        name: "FK_DEPARTMENT_TEACHER_HeadTeacherId",
                        column: x => x.HeadTeacherId,
                        principalTable: "TEACHER",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TEACHER_DepartmentId",
                table: "TEACHER",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DEPARTMENT_HeadTeacherId",
                table: "DEPARTMENT",
                column: "HeadTeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_TEACHER_DEPARTMENT_DepartmentId",
                table: "TEACHER",
                column: "DepartmentId",
                principalTable: "DEPARTMENT",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TEACHER_DEPARTMENT_DepartmentId",
                table: "TEACHER");

            migrationBuilder.DropTable(
                name: "DEPARTMENT");

            migrationBuilder.DropIndex(
                name: "IX_TEACHER_DepartmentId",
                table: "TEACHER");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "TEACHER");

            migrationBuilder.AddColumn<bool>(
                name: "IsDepartmentHead",
                table: "TEACHER",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
