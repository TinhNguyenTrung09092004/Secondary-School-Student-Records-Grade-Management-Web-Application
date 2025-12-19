using Microsoft.EntityFrameworkCore;
using API.Data;
using API.DTOs;
using API.Models;

namespace API.Services;

public class ConductGradingService : IConductGradingService
{
    private readonly ApplicationDbContext _context;

    public ConductGradingService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Tính kết quả rèn luyện cả năm học dựa trên quy định:
    /// - Tốt: HK2 = Tốt và HK1 >= Khá
    /// - Khá: HK2 = Khá và HK1 >= Đạt; hoặc HK2 = Đạt và HK1 = Tốt; hoặc HK2 = Tốt và HK1 ∈ {Đạt, Chưa đạt}
    /// - Đạt: HK2 = Đạt và HK1 ∈ {Khá, Đạt, Chưa đạt}; hoặc HK2 = Khá và HK1 = Chưa đạt
    /// - Chưa đạt: Các trường hợp còn lại
    /// </summary>
    private string? CalculateYearEndConductId(string? hk1ConductName, string? hk2ConductName, List<Conduct> conductList)
    {
        if (string.IsNullOrEmpty(hk1ConductName) || string.IsNullOrEmpty(hk2ConductName))
        {
            return null;
        }

        string resultName;

        // Quy tắc 1: Tốt
        if (hk2ConductName == "Tốt" && (hk1ConductName == "Tốt" || hk1ConductName == "Khá"))
        {
            resultName = "Tốt";
        }
        // Quy tắc 2: Khá
        else if (hk2ConductName == "Khá" && (hk1ConductName == "Khá" || hk1ConductName == "Đạt" || hk1ConductName == "Tốt"))
        {
            resultName = "Khá";
        }
        else if (hk2ConductName == "Đạt" && hk1ConductName == "Tốt")
        {
            resultName = "Khá";
        }
        else if (hk2ConductName == "Tốt" && (hk1ConductName == "Đạt" || hk1ConductName == "Chưa đạt"))
        {
            resultName = "Khá";
        }
        // Quy tắc 3: Đạt
        else if (hk2ConductName == "Đạt" && (hk1ConductName == "Khá" || hk1ConductName == "Đạt" || hk1ConductName == "Chưa đạt"))
        {
            resultName = "Đạt";
        }
        else if (hk2ConductName == "Khá" && hk1ConductName == "Chưa đạt")
        {
            resultName = "Đạt";
        }
        // Quy tắc 4: Chưa đạt
        else
        {
            resultName = "Chưa đạt";
        }

        // Find the conduct ID by name
        return conductList.FirstOrDefault(c => c.ConductName == resultName)?.ConductId;
    }

    public async Task<List<ClassForConductGradingDto>> GetTeacherClassesAsync(string teacherId)
    {
        var classes = await _context.Classes
            .Where(c => c.TeacherId == teacherId)
            .Include(c => c.SchoolYear)
            .Include(c => c.ClassAssignments)
            .Select(c => new ClassForConductGradingDto
            {
                ClassId = c.ClassId,
                ClassName = c.ClassName,
                SchoolYearId = c.SchoolYearId,
                SchoolYearName = c.SchoolYear.SchoolYearName,
                StudentCount = c.ClassAssignments.Count
            })
            .OrderByDescending(c => c.SchoolYearId)
            .ThenBy(c => c.ClassName)
            .ToListAsync();

        return classes;
    }

    public async Task<List<StudentConductDto>> GetStudentsForConductGradingAsync(string classId, string schoolYearId, string teacherId)
    {
        // Verify that the teacher is the homeroom teacher of this class
        var classEntity = await _context.Classes
            .FirstOrDefaultAsync(c => c.ClassId == classId && c.SchoolYearId == schoolYearId);

        if (classEntity == null || classEntity.TeacherId != teacherId)
        {
            return new List<StudentConductDto>();
        }

        // Get students in the class with their semester conduct grades
        var students = await _context.ClassAssignments
            .Where(ca => ca.ClassId == classId && ca.SchoolYearId == schoolYearId)
            .Include(ca => ca.Student)
            .Select(ca => new
            {
                ca.StudentId,
                ca.Student.FullName,
                ca.ClassId,
                ClassEntity = ca.Class,
                ca.SchoolYearId,
                Semester1Result = _context.StudentSemesterResults
                    .Where(ssr => ssr.StudentId == ca.StudentId &&
                                 ssr.ClassId == ca.ClassId &&
                                 ssr.SchoolYearId == ca.SchoolYearId &&
                                 ssr.SemesterId == "HK1")
                    .Include(ssr => ssr.Conduct)
                    .FirstOrDefault(),
                Semester2Result = _context.StudentSemesterResults
                    .Where(ssr => ssr.StudentId == ca.StudentId &&
                                 ssr.ClassId == ca.ClassId &&
                                 ssr.SchoolYearId == ca.SchoolYearId &&
                                 ssr.SemesterId == "HK2")
                    .Include(ssr => ssr.Conduct)
                    .FirstOrDefault(),
                YearResult = _context.StudentYearResults
                    .Where(syr => syr.StudentId == ca.StudentId &&
                                 syr.ClassId == ca.ClassId &&
                                 syr.SchoolYearId == ca.SchoolYearId)
                    .Include(syr => syr.Conduct)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return students.Select(s => new StudentConductDto
        {
            StudentId = s.StudentId,
            StudentName = s.FullName,
            ClassId = s.ClassId,
            ClassName = s.ClassEntity.ClassName,
            SchoolYearId = s.SchoolYearId,
            Semester1ConductId = s.Semester1Result?.ConductId,
            Semester1ConductName = s.Semester1Result?.Conduct?.ConductName,
            Semester2ConductId = s.Semester2Result?.ConductId,
            Semester2ConductName = s.Semester2Result?.Conduct?.ConductName,
            YearEndConductId = s.YearResult?.ConductId,
            YearEndConductName = s.YearResult?.Conduct?.ConductName
        })
        .OrderBy(s => s.StudentName)
        .ToList();
    }

    public async Task<bool> UpdateStudentConductAsync(UpdateStudentConductDto updateDto, string teacherId)
    {
        // Verify that the teacher is the homeroom teacher of this class
        var classEntity = await _context.Classes
            .FirstOrDefaultAsync(c => c.ClassId == updateDto.ClassId && c.SchoolYearId == updateDto.SchoolYearId);

        if (classEntity == null)
        {
            throw new Exception($"Không tìm thấy lớp {updateDto.ClassId} trong năm học {updateDto.SchoolYearId}");
        }

        if (classEntity.TeacherId != teacherId)
        {
            throw new Exception($"Bạn không phải giáo viên chủ nhiệm của lớp {updateDto.ClassId}. GVCN hiện tại: {classEntity.TeacherId}, TeacherId của bạn: {teacherId}");
        }

        // Verify that the conduct exists
        var conductExists = await _context.Set<Conduct>().AnyAsync(c => c.ConductId == updateDto.ConductId);
        if (!conductExists)
        {
            throw new Exception($"Không tìm thấy hạnh kiểm với mã {updateDto.ConductId}");
        }

        // Get or create StudentSemesterResult
        var semesterResult = await _context.StudentSemesterResults
            .FirstOrDefaultAsync(ssr => ssr.StudentId == updateDto.StudentId &&
                                       ssr.ClassId == updateDto.ClassId &&
                                       ssr.SchoolYearId == updateDto.SchoolYearId &&
                                       ssr.SemesterId == updateDto.SemesterId);

        if (semesterResult != null)
        {
            // Update existing record
            semesterResult.ConductId = updateDto.ConductId;
            _context.Entry(semesterResult).State = EntityState.Modified;
        }
        else
        {
            // Create new record
            semesterResult = new StudentSemesterResult
            {
                StudentId = updateDto.StudentId,
                ClassId = updateDto.ClassId,
                SchoolYearId = updateDto.SchoolYearId,
                SemesterId = updateDto.SemesterId,
                ConductId = updateDto.ConductId,
                AverageSemester = 0
            };
            _context.StudentSemesterResults.Add(semesterResult);
        }

        await _context.SaveChangesAsync();

        // Calculate and update year-end conduct
        await UpdateYearEndConductAsync(updateDto.StudentId, updateDto.ClassId, updateDto.SchoolYearId);

        return true;
    }

    /// <summary>
    /// Tính toán và cập nhật kết quả rèn luyện cả năm cho học sinh
    /// </summary>
    private async Task UpdateYearEndConductAsync(string studentId, string classId, string schoolYearId)
    {
        // Get both semester results
        var semesterResults = await _context.StudentSemesterResults
            .Where(ssr => ssr.StudentId == studentId &&
                         ssr.ClassId == classId &&
                         ssr.SchoolYearId == schoolYearId)
            .Include(ssr => ssr.Conduct)
            .ToListAsync();

        var hk1Result = semesterResults.FirstOrDefault(sr => sr.SemesterId == "HK1");
        var hk2Result = semesterResults.FirstOrDefault(sr => sr.SemesterId == "HK2");

        // Only calculate if both semesters have conduct grades
        if (hk1Result?.Conduct == null || hk2Result?.Conduct == null)
        {
            return;
        }

        // Get all conduct types
        var conductList = await _context.Set<Conduct>().ToListAsync();

        // Calculate year-end conduct
        var yearEndConductId = CalculateYearEndConductId(
            hk1Result.Conduct.ConductName,
            hk2Result.Conduct.ConductName,
            conductList
        );

        if (yearEndConductId == null)
        {
            return;
        }

        // Get or create StudentYearResult
        var yearResult = await _context.StudentYearResults
            .FirstOrDefaultAsync(syr => syr.StudentId == studentId &&
                                       syr.ClassId == classId &&
                                       syr.SchoolYearId == schoolYearId);

        if (yearResult != null)
        {
            // Update existing record
            yearResult.ConductId = yearEndConductId;
            _context.Entry(yearResult).State = EntityState.Modified;
        }
        else
        {
            // Create new record - we need to provide default values for other required fields
            // Note: This might need adjustment based on your business logic for other fields
            var defaultAcademicPerformance = await _context.Set<AcademicPerformance>().FirstOrDefaultAsync();
            var defaultResult = await _context.Set<Result>().FirstOrDefaultAsync();

            if (defaultAcademicPerformance == null || defaultResult == null)
            {
                // Cannot create year result without required reference data
                return;
            }

            yearResult = new StudentYearResult
            {
                StudentId = studentId,
                ClassId = classId,
                SchoolYearId = schoolYearId,
                ConductId = yearEndConductId,
                AcademicPerformanceId = defaultAcademicPerformance.AcademicPerformanceId,
                ResultId = defaultResult.ResultId,
                AverageSemester1 = 0,
                AverageSemester2 = 0,
                AverageYear = 0
            };
            _context.StudentYearResults.Add(yearResult);
        }

        await _context.SaveChangesAsync();
    }
}
