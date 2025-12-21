using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Ethnicity> Ethnicities { get; set; }
    public DbSet<Religion> Religions { get; set; }
    public DbSet<SchoolYear> SchoolYears { get; set; }
    public DbSet<Semester> Semesters { get; set; }
    public DbSet<GradeLevel> GradeLevels { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<AcademicPerformance> AcademicPerformances { get; set; }
    public DbSet<Conduct> Conducts { get; set; }
    public DbSet<Result> Results { get; set; }
    public DbSet<Occupation> Occupations { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<ClassAssignment> ClassAssignments { get; set; }
    public DbSet<TeachingAssignment> TeachingAssignments { get; set; }
    public DbSet<GradeType> GradeTypes { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<StudentSubjectResult> StudentSubjectResults { get; set; }
    public DbSet<StudentSemesterResult> StudentSemesterResults { get; set; }
    public DbSet<StudentYearResult> StudentYearResults { get; set; }
    public DbSet<ClassSubjectResult> ClassSubjectResults { get; set; }
    public DbSet<ClassSemesterResult> ClassSemesterResults { get; set; }
    public DbSet<Regulation> Regulations { get; set; }
    public DbSet<BackupSchedule> BackupSchedules { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ClassAssignment>().HasKey(p => new { p.SchoolYearId, p.GradeLevelId, p.ClassId, p.StudentId });
        modelBuilder.Entity<StudentSubjectResult>().HasKey(k => new { k.StudentId, k.ClassId, k.SchoolYearId, k.SubjectId, k.SemesterId });
        modelBuilder.Entity<StudentSemesterResult>().HasKey(k => new { k.StudentId, k.ClassId, k.SchoolYearId, k.SemesterId });
        modelBuilder.Entity<StudentYearResult>().HasKey(k => new { k.StudentId, k.ClassId, k.SchoolYearId });
        modelBuilder.Entity<ClassSubjectResult>().HasKey(k => new { k.ClassId, k.SchoolYearId, k.SubjectId, k.SemesterId });
        modelBuilder.Entity<ClassSemesterResult>().HasKey(k => new { k.ClassId, k.SchoolYearId, k.SemesterId });

        modelBuilder.Entity<Regulation>().HasNoKey();
        modelBuilder.Entity<SystemSetting>().HasKey(s => s.Key);

        // Configure Department-Teacher relationship to avoid circular reference
        modelBuilder.Entity<Department>()
            .HasOne(d => d.HeadTeacher)
            .WithMany()
            .HasForeignKey(d => d.HeadTeacherId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Teacher>()
            .HasOne(t => t.Department)
            .WithMany(d => d.Teachers)
            .HasForeignKey(t => t.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}