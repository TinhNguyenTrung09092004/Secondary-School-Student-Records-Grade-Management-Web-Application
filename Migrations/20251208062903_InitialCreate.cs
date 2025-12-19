using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ACADEMIC_PERFORMANCE",
                columns: table => new
                {
                    AcademicPerformanceId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    AcademicPerformanceName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    MinRequiredScore = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    MaxRequiredScore = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    ControlScore = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ACADEMIC_PERFORMANCE", x => x.AcademicPerformanceId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ScheduledDeletionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsAccountSetupComplete = table.Column<bool>(type: "boolean", nullable: false),
                    AccountSetupToken = table.Column<string>(type: "text", nullable: true),
                    AccountSetupTokenExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BackupSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BackupTime = table.Column<string>(type: "text", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LastBackupDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackupSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CONDUCT",
                columns: table => new
                {
                    ConductId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    ConductName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONDUCT", x => x.ConductId);
                });

            migrationBuilder.CreateTable(
                name: "ETHNICITY",
                columns: table => new
                {
                    EthnicityId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    EthnicityName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ETHNICITY", x => x.EthnicityId);
                });

            migrationBuilder.CreateTable(
                name: "GRADE_LEVEL",
                columns: table => new
                {
                    GradeLevelId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    GradeLevelName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRADE_LEVEL", x => x.GradeLevelId);
                });

            migrationBuilder.CreateTable(
                name: "GRADE_TYPE",
                columns: table => new
                {
                    GradeTypeId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    GradeTypeName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Coefficient = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRADE_TYPE", x => x.GradeTypeId);
                });

            migrationBuilder.CreateTable(
                name: "OCCUPATION",
                columns: table => new
                {
                    OccupationId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    OccupationName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OCCUPATION", x => x.OccupationId);
                });

            migrationBuilder.CreateTable(
                name: "REGULATION",
                columns: table => new
                {
                    MinClassSize = table.Column<int>(type: "integer", nullable: false),
                    MaxClassSize = table.Column<int>(type: "integer", nullable: false),
                    PassingScore = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "RELIGION",
                columns: table => new
                {
                    ReligionId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    ReligionName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RELIGION", x => x.ReligionId);
                });

            migrationBuilder.CreateTable(
                name: "RESULT",
                columns: table => new
                {
                    ResultId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    ResultName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RESULT", x => x.ResultId);
                });

            migrationBuilder.CreateTable(
                name: "SCHOOL_YEAR",
                columns: table => new
                {
                    SchoolYearId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    SchoolYearName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCHOOL_YEAR", x => x.SchoolYearId);
                });

            migrationBuilder.CreateTable(
                name: "SEMESTER",
                columns: table => new
                {
                    SemesterId = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    SemesterName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Coefficient = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SEMESTER", x => x.SemesterId);
                });

            migrationBuilder.CreateTable(
                name: "SUBJECT",
                columns: table => new
                {
                    SubjectId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    SubjectName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    LessonCount = table.Column<int>(type: "integer", nullable: false),
                    Coefficient = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SUBJECT", x => x.SubjectId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "STUDENT",
                columns: table => new
                {
                    StudentId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    FullName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Gender = table.Column<bool>(type: "boolean", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Address = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EthnicityId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    ReligionId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    FatherName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    FatherOccupationId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    MotherName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    MotherOccupationId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    Email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STUDENT", x => x.StudentId);
                    table.ForeignKey(
                        name: "FK_STUDENT_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_STUDENT_ETHNICITY_EthnicityId",
                        column: x => x.EthnicityId,
                        principalTable: "ETHNICITY",
                        principalColumn: "EthnicityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STUDENT_OCCUPATION_FatherOccupationId",
                        column: x => x.FatherOccupationId,
                        principalTable: "OCCUPATION",
                        principalColumn: "OccupationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STUDENT_OCCUPATION_MotherOccupationId",
                        column: x => x.MotherOccupationId,
                        principalTable: "OCCUPATION",
                        principalColumn: "OccupationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STUDENT_RELIGION_ReligionId",
                        column: x => x.ReligionId,
                        principalTable: "RELIGION",
                        principalColumn: "ReligionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TEACHER",
                columns: table => new
                {
                    TeacherId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    TeacherName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Address = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    IsDepartmentHead = table.Column<bool>(type: "boolean", nullable: false),
                    SubjectId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TEACHER", x => x.TeacherId);
                    table.ForeignKey(
                        name: "FK_TEACHER_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TEACHER_SUBJECT_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "SUBJECT",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CLASS",
                columns: table => new
                {
                    ClassId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ClassName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    GradeLevelId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    SchoolYearId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    ClassSize = table.Column<int>(type: "integer", nullable: false),
                    TeacherId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLASS", x => x.ClassId);
                    table.ForeignKey(
                        name: "FK_CLASS_GRADE_LEVEL_GradeLevelId",
                        column: x => x.GradeLevelId,
                        principalTable: "GRADE_LEVEL",
                        principalColumn: "GradeLevelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CLASS_SCHOOL_YEAR_SchoolYearId",
                        column: x => x.SchoolYearId,
                        principalTable: "SCHOOL_YEAR",
                        principalColumn: "SchoolYearId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CLASS_TEACHER_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "TEACHER",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CLASS_ASSIGNMENT",
                columns: table => new
                {
                    SchoolYearId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    GradeLevelId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    ClassId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    StudentId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLASS_ASSIGNMENT", x => new { x.SchoolYearId, x.GradeLevelId, x.ClassId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_CLASS_ASSIGNMENT_CLASS_ClassId",
                        column: x => x.ClassId,
                        principalTable: "CLASS",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CLASS_ASSIGNMENT_GRADE_LEVEL_GradeLevelId",
                        column: x => x.GradeLevelId,
                        principalTable: "GRADE_LEVEL",
                        principalColumn: "GradeLevelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CLASS_ASSIGNMENT_SCHOOL_YEAR_SchoolYearId",
                        column: x => x.SchoolYearId,
                        principalTable: "SCHOOL_YEAR",
                        principalColumn: "SchoolYearId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CLASS_ASSIGNMENT_STUDENT_StudentId",
                        column: x => x.StudentId,
                        principalTable: "STUDENT",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CLASS_SEMESTER_RESULT",
                columns: table => new
                {
                    ClassId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SchoolYearId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    SemesterId = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    PassCount = table.Column<int>(type: "integer", nullable: false),
                    PassRate = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLASS_SEMESTER_RESULT", x => new { x.ClassId, x.SchoolYearId, x.SemesterId });
                    table.ForeignKey(
                        name: "FK_CLASS_SEMESTER_RESULT_CLASS_ClassId",
                        column: x => x.ClassId,
                        principalTable: "CLASS",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CLASS_SEMESTER_RESULT_SCHOOL_YEAR_SchoolYearId",
                        column: x => x.SchoolYearId,
                        principalTable: "SCHOOL_YEAR",
                        principalColumn: "SchoolYearId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CLASS_SEMESTER_RESULT_SEMESTER_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "SEMESTER",
                        principalColumn: "SemesterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CLASS_SUBJECT_RESULT",
                columns: table => new
                {
                    ClassId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SchoolYearId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    SubjectId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    SemesterId = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    PassCount = table.Column<int>(type: "integer", nullable: false),
                    PassRate = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLASS_SUBJECT_RESULT", x => new { x.ClassId, x.SchoolYearId, x.SubjectId, x.SemesterId });
                    table.ForeignKey(
                        name: "FK_CLASS_SUBJECT_RESULT_CLASS_ClassId",
                        column: x => x.ClassId,
                        principalTable: "CLASS",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CLASS_SUBJECT_RESULT_SCHOOL_YEAR_SchoolYearId",
                        column: x => x.SchoolYearId,
                        principalTable: "SCHOOL_YEAR",
                        principalColumn: "SchoolYearId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CLASS_SUBJECT_RESULT_SEMESTER_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "SEMESTER",
                        principalColumn: "SemesterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CLASS_SUBJECT_RESULT_SUBJECT_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "SUBJECT",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GRADE",
                columns: table => new
                {
                    RowNumber = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    SubjectId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    SemesterId = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    SchoolYearId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    ClassId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    GradeTypeId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    Score = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRADE", x => x.RowNumber);
                    table.ForeignKey(
                        name: "FK_GRADE_CLASS_ClassId",
                        column: x => x.ClassId,
                        principalTable: "CLASS",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GRADE_GRADE_TYPE_GradeTypeId",
                        column: x => x.GradeTypeId,
                        principalTable: "GRADE_TYPE",
                        principalColumn: "GradeTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GRADE_SCHOOL_YEAR_SchoolYearId",
                        column: x => x.SchoolYearId,
                        principalTable: "SCHOOL_YEAR",
                        principalColumn: "SchoolYearId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GRADE_SEMESTER_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "SEMESTER",
                        principalColumn: "SemesterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GRADE_STUDENT_StudentId",
                        column: x => x.StudentId,
                        principalTable: "STUDENT",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GRADE_SUBJECT_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "SUBJECT",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "STUDENT_SUBJECT_RESULT",
                columns: table => new
                {
                    StudentId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    ClassId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SchoolYearId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    SubjectId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    SemesterId = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    AverageOral = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Average15Min = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Average45Min = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    ExamScore = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    AverageSemester = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STUDENT_SUBJECT_RESULT", x => new { x.StudentId, x.ClassId, x.SchoolYearId, x.SubjectId, x.SemesterId });
                    table.ForeignKey(
                        name: "FK_STUDENT_SUBJECT_RESULT_CLASS_ClassId",
                        column: x => x.ClassId,
                        principalTable: "CLASS",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STUDENT_SUBJECT_RESULT_SCHOOL_YEAR_SchoolYearId",
                        column: x => x.SchoolYearId,
                        principalTable: "SCHOOL_YEAR",
                        principalColumn: "SchoolYearId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STUDENT_SUBJECT_RESULT_SEMESTER_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "SEMESTER",
                        principalColumn: "SemesterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STUDENT_SUBJECT_RESULT_STUDENT_StudentId",
                        column: x => x.StudentId,
                        principalTable: "STUDENT",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STUDENT_SUBJECT_RESULT_SUBJECT_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "SUBJECT",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "STUDENT_YEAR_RESULT",
                columns: table => new
                {
                    StudentId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    ClassId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SchoolYearId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    AcademicPerformanceId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    ConductId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    ResultId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    AverageSemester1 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    AverageSemester2 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    AverageYear = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STUDENT_YEAR_RESULT", x => new { x.StudentId, x.ClassId, x.SchoolYearId });
                    table.ForeignKey(
                        name: "FK_STUDENT_YEAR_RESULT_ACADEMIC_PERFORMANCE_AcademicPerformanc~",
                        column: x => x.AcademicPerformanceId,
                        principalTable: "ACADEMIC_PERFORMANCE",
                        principalColumn: "AcademicPerformanceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STUDENT_YEAR_RESULT_CLASS_ClassId",
                        column: x => x.ClassId,
                        principalTable: "CLASS",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STUDENT_YEAR_RESULT_CONDUCT_ConductId",
                        column: x => x.ConductId,
                        principalTable: "CONDUCT",
                        principalColumn: "ConductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STUDENT_YEAR_RESULT_RESULT_ResultId",
                        column: x => x.ResultId,
                        principalTable: "RESULT",
                        principalColumn: "ResultId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STUDENT_YEAR_RESULT_SCHOOL_YEAR_SchoolYearId",
                        column: x => x.SchoolYearId,
                        principalTable: "SCHOOL_YEAR",
                        principalColumn: "SchoolYearId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STUDENT_YEAR_RESULT_STUDENT_StudentId",
                        column: x => x.StudentId,
                        principalTable: "STUDENT",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TEACHING_ASSIGNMENT",
                columns: table => new
                {
                    RowNumber = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchoolYearId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    ClassId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SubjectId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    TeacherId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TEACHING_ASSIGNMENT", x => x.RowNumber);
                    table.ForeignKey(
                        name: "FK_TEACHING_ASSIGNMENT_CLASS_ClassId",
                        column: x => x.ClassId,
                        principalTable: "CLASS",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TEACHING_ASSIGNMENT_SCHOOL_YEAR_SchoolYearId",
                        column: x => x.SchoolYearId,
                        principalTable: "SCHOOL_YEAR",
                        principalColumn: "SchoolYearId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TEACHING_ASSIGNMENT_SUBJECT_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "SUBJECT",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TEACHING_ASSIGNMENT_TEACHER_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "TEACHER",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CLASS_GradeLevelId",
                table: "CLASS",
                column: "GradeLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_CLASS_SchoolYearId",
                table: "CLASS",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_CLASS_TeacherId",
                table: "CLASS",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_CLASS_ASSIGNMENT_ClassId",
                table: "CLASS_ASSIGNMENT",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CLASS_ASSIGNMENT_GradeLevelId",
                table: "CLASS_ASSIGNMENT",
                column: "GradeLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_CLASS_ASSIGNMENT_StudentId",
                table: "CLASS_ASSIGNMENT",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_CLASS_SEMESTER_RESULT_SchoolYearId",
                table: "CLASS_SEMESTER_RESULT",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_CLASS_SEMESTER_RESULT_SemesterId",
                table: "CLASS_SEMESTER_RESULT",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_CLASS_SUBJECT_RESULT_SchoolYearId",
                table: "CLASS_SUBJECT_RESULT",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_CLASS_SUBJECT_RESULT_SemesterId",
                table: "CLASS_SUBJECT_RESULT",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_CLASS_SUBJECT_RESULT_SubjectId",
                table: "CLASS_SUBJECT_RESULT",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_GRADE_ClassId",
                table: "GRADE",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_GRADE_GradeTypeId",
                table: "GRADE",
                column: "GradeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GRADE_SchoolYearId",
                table: "GRADE",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_GRADE_SemesterId",
                table: "GRADE",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_GRADE_StudentId",
                table: "GRADE",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_GRADE_SubjectId",
                table: "GRADE",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_EthnicityId",
                table: "STUDENT",
                column: "EthnicityId");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_FatherOccupationId",
                table: "STUDENT",
                column: "FatherOccupationId");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_MotherOccupationId",
                table: "STUDENT",
                column: "MotherOccupationId");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_ReligionId",
                table: "STUDENT",
                column: "ReligionId");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_UserId",
                table: "STUDENT",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_SUBJECT_RESULT_ClassId",
                table: "STUDENT_SUBJECT_RESULT",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_SUBJECT_RESULT_SchoolYearId",
                table: "STUDENT_SUBJECT_RESULT",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_SUBJECT_RESULT_SemesterId",
                table: "STUDENT_SUBJECT_RESULT",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_SUBJECT_RESULT_SubjectId",
                table: "STUDENT_SUBJECT_RESULT",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_YEAR_RESULT_AcademicPerformanceId",
                table: "STUDENT_YEAR_RESULT",
                column: "AcademicPerformanceId");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_YEAR_RESULT_ClassId",
                table: "STUDENT_YEAR_RESULT",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_YEAR_RESULT_ConductId",
                table: "STUDENT_YEAR_RESULT",
                column: "ConductId");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_YEAR_RESULT_ResultId",
                table: "STUDENT_YEAR_RESULT",
                column: "ResultId");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_YEAR_RESULT_SchoolYearId",
                table: "STUDENT_YEAR_RESULT",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_TEACHER_SubjectId",
                table: "TEACHER",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TEACHER_UserId",
                table: "TEACHER",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TEACHING_ASSIGNMENT_ClassId",
                table: "TEACHING_ASSIGNMENT",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TEACHING_ASSIGNMENT_SchoolYearId",
                table: "TEACHING_ASSIGNMENT",
                column: "SchoolYearId");

            migrationBuilder.CreateIndex(
                name: "IX_TEACHING_ASSIGNMENT_SubjectId",
                table: "TEACHING_ASSIGNMENT",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TEACHING_ASSIGNMENT_TeacherId",
                table: "TEACHING_ASSIGNMENT",
                column: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BackupSchedules");

            migrationBuilder.DropTable(
                name: "CLASS_ASSIGNMENT");

            migrationBuilder.DropTable(
                name: "CLASS_SEMESTER_RESULT");

            migrationBuilder.DropTable(
                name: "CLASS_SUBJECT_RESULT");

            migrationBuilder.DropTable(
                name: "GRADE");

            migrationBuilder.DropTable(
                name: "REGULATION");

            migrationBuilder.DropTable(
                name: "STUDENT_SUBJECT_RESULT");

            migrationBuilder.DropTable(
                name: "STUDENT_YEAR_RESULT");

            migrationBuilder.DropTable(
                name: "TEACHING_ASSIGNMENT");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "GRADE_TYPE");

            migrationBuilder.DropTable(
                name: "SEMESTER");

            migrationBuilder.DropTable(
                name: "ACADEMIC_PERFORMANCE");

            migrationBuilder.DropTable(
                name: "CONDUCT");

            migrationBuilder.DropTable(
                name: "RESULT");

            migrationBuilder.DropTable(
                name: "STUDENT");

            migrationBuilder.DropTable(
                name: "CLASS");

            migrationBuilder.DropTable(
                name: "ETHNICITY");

            migrationBuilder.DropTable(
                name: "OCCUPATION");

            migrationBuilder.DropTable(
                name: "RELIGION");

            migrationBuilder.DropTable(
                name: "GRADE_LEVEL");

            migrationBuilder.DropTable(
                name: "SCHOOL_YEAR");

            migrationBuilder.DropTable(
                name: "TEACHER");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "SUBJECT");
        }
    }
}
