using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class IdTeacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CLASS_TEACHER_TeacherId",
                table: "CLASS");

            migrationBuilder.AlterColumn<string>(
                name: "TeacherId",
                table: "CLASS",
                type: "character varying(6)",
                maxLength: 6,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6);

            migrationBuilder.AddForeignKey(
                name: "FK_CLASS_TEACHER_TeacherId",
                table: "CLASS",
                column: "TeacherId",
                principalTable: "TEACHER",
                principalColumn: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CLASS_TEACHER_TeacherId",
                table: "CLASS");

            migrationBuilder.AlterColumn<string>(
                name: "TeacherId",
                table: "CLASS",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CLASS_TEACHER_TeacherId",
                table: "CLASS",
                column: "TeacherId",
                principalTable: "TEACHER",
                principalColumn: "TeacherId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
