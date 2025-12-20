using API.DTOs;
using API.Models;
using API.Repositories;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Xceed.Words.NET;
using Xceed.Document.NET;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Drawing;

namespace API.Services;

public class GradeService : IGradeService
{
    private readonly IGradeRepository _gradeRepository;
    private readonly ITeachingAssignmentRepository _teachingAssignmentRepository;
    private readonly IClassAssignmentRepository _classAssignmentRepository;
    private readonly IGradeTypeRepository _gradeTypeRepository;
    private readonly ISemesterRepository _semesterRepository;
    private readonly IStudentSubjectResultService _studentSubjectResultService;
    private readonly ApplicationDbContext _context;

    public GradeService(
        IGradeRepository gradeRepository,
        ITeachingAssignmentRepository teachingAssignmentRepository,
        IClassAssignmentRepository classAssignmentRepository,
        IGradeTypeRepository gradeTypeRepository,
        ISemesterRepository semesterRepository,
        IStudentSubjectResultService studentSubjectResultService,
        ApplicationDbContext context)
    {
        _gradeRepository = gradeRepository;
        _teachingAssignmentRepository = teachingAssignmentRepository;
        _classAssignmentRepository = classAssignmentRepository;
        _gradeTypeRepository = gradeTypeRepository;
        _semesterRepository = semesterRepository;
        _studentSubjectResultService = studentSubjectResultService;
        _context = context;
    }

    public async Task<IEnumerable<TeacherClassSubjectDto>> GetTeacherClassSubjectsAsync(string teacherId)
    {
        var assignments = await _teachingAssignmentRepository.GetAssignmentsByTeacherIdAsync(teacherId);

        return assignments.Select(a => new TeacherClassSubjectDto
        {
            ClassId = a.ClassId,
            ClassName = a.Class.ClassName,
            SubjectId = a.SubjectId,
            SubjectName = a.Subject.SubjectName,
            SchoolYearId = a.SchoolYearId,
            SchoolYearName = a.SchoolYear.SchoolYearName
        }).ToList();
    }

    public async Task<IEnumerable<StudentGradeDto>> GetStudentGradesForClassSubjectAsync(
        string classId, string subjectId, string semesterId, string schoolYearId)
    {
        var classAssignments = await _classAssignmentRepository.GetStudentsByClassIdAsync(classId);
        var gradeTypes = await _gradeTypeRepository.GetAllAsync();
        var grades = await _gradeRepository.GetGradesByClassSubjectSemesterAsync(classId, subjectId, semesterId, schoolYearId);

        var studentGrades = new List<StudentGradeDto>();

        foreach (var assignment in classAssignments)
        {
            var studentGradeDto = new StudentGradeDto
            {
                StudentId = assignment.StudentId,
                StudentName = assignment.Student.FullName,
                Grades = new List<GradeDetailDto>()
            };

            foreach (var gradeType in gradeTypes)
            {
                var existingGrade = grades.FirstOrDefault(g =>
                    g.StudentId == assignment.StudentId &&
                    g.GradeTypeId == gradeType.GradeTypeId);

                studentGradeDto.Grades.Add(new GradeDetailDto
                {
                    RowNumber = existingGrade?.RowNumber ?? 0,
                    GradeTypeId = gradeType.GradeTypeId,
                    GradeTypeName = gradeType.GradeTypeName,
                    Coefficient = gradeType.Coefficient,
                    Score = existingGrade?.Score,
                    IsComment = existingGrade?.IsComment ?? false,
                    Comment = existingGrade?.Comment
                });
            }

            studentGrades.Add(studentGradeDto);
        }

        return studentGrades;
    }

    public async Task<GradeDto> CreateGradeAsync(CreateGradeDto createGradeDto)
    {
        var existingGrade = await _gradeRepository.GetGradeByDetailsAsync(
            createGradeDto.StudentId,
            createGradeDto.SubjectId,
            createGradeDto.SemesterId,
            createGradeDto.SchoolYearId,
            createGradeDto.ClassId,
            createGradeDto.GradeTypeId);

        if (existingGrade != null)
        {
            throw new InvalidOperationException("Điểm này đã tồn tại");
        }

        var grade = new Grade
        {
            StudentId = createGradeDto.StudentId,
            SubjectId = createGradeDto.SubjectId,
            SemesterId = createGradeDto.SemesterId,
            SchoolYearId = createGradeDto.SchoolYearId,
            ClassId = createGradeDto.ClassId,
            GradeTypeId = createGradeDto.GradeTypeId,
            Score = createGradeDto.Score,
            IsComment = createGradeDto.IsComment,
            Comment = createGradeDto.Comment
        };

        var created = await _gradeRepository.AddAsync(grade);

        // Automatically calculate and save results after grade is created
        await _studentSubjectResultService.CalculateAndSaveCompleteSemesterAsync(
            created.StudentId, created.ClassId, created.SchoolYearId, created.SemesterId);

        // Try to calculate year summary (will only succeed if both semesters have data)
        try
        {
            await _studentSubjectResultService.CalculateAndSaveYearSummaryAsync(
                created.StudentId, created.ClassId, created.SchoolYearId);
        }
        catch
        {
            // Year summary calculation failed (likely missing data from other semester)
            // This is expected and can be ignored
        }

        return new GradeDto
        {
            RowNumber = created.RowNumber,
            StudentId = created.StudentId,
            SubjectId = created.SubjectId,
            SemesterId = created.SemesterId,
            SchoolYearId = created.SchoolYearId,
            ClassId = created.ClassId,
            GradeTypeId = created.GradeTypeId,
            Score = created.Score,
            IsComment = created.IsComment,
            Comment = created.Comment
        };
    }

    public async Task<GradeDto> UpdateGradeAsync(int rowNumber, UpdateGradeDto updateGradeDto)
    {
        var grade = await _gradeRepository.GetByIdAsync(rowNumber);
        if (grade == null)
        {
            throw new KeyNotFoundException("Không tìm thấy điểm");
        }

        grade.Score = updateGradeDto.Score;
        grade.IsComment = updateGradeDto.IsComment;
        grade.Comment = updateGradeDto.Comment;
        await _gradeRepository.UpdateAsync(grade);

        // Automatically recalculate results after grade is updated
        await _studentSubjectResultService.CalculateAndSaveCompleteSemesterAsync(
            grade.StudentId, grade.ClassId, grade.SchoolYearId, grade.SemesterId);

        // Try to calculate year summary (will only succeed if both semesters have data)
        try
        {
            await _studentSubjectResultService.CalculateAndSaveYearSummaryAsync(
                grade.StudentId, grade.ClassId, grade.SchoolYearId);
        }
        catch
        {
            // Year summary calculation failed (likely missing data from other semester)
            // This is expected and can be ignored
        }

        return new GradeDto
        {
            RowNumber = grade.RowNumber,
            StudentId = grade.StudentId,
            SubjectId = grade.SubjectId,
            SemesterId = grade.SemesterId,
            SchoolYearId = grade.SchoolYearId,
            ClassId = grade.ClassId,
            GradeTypeId = grade.GradeTypeId,
            Score = grade.Score,
            IsComment = grade.IsComment,
            Comment = grade.Comment
        };
    }

    public async Task DeleteGradeAsync(int rowNumber)
    {
        var grade = await _gradeRepository.GetByIdAsync(rowNumber);
        if (grade == null)
        {
            throw new KeyNotFoundException("Không tìm thấy điểm");
        }

        // Save grade info before deleting
        var studentId = grade.StudentId;
        var classId = grade.ClassId;
        var schoolYearId = grade.SchoolYearId;
        var semesterId = grade.SemesterId;

        await _gradeRepository.DeleteAsync(rowNumber);

        // Recalculate results after grade is deleted
        await _studentSubjectResultService.CalculateAndSaveCompleteSemesterAsync(
            studentId, classId, schoolYearId, semesterId);

        // Try to calculate year summary (will only succeed if both semesters have data)
        try
        {
            await _studentSubjectResultService.CalculateAndSaveYearSummaryAsync(
                studentId, classId, schoolYearId);
        }
        catch
        {
            // Year summary calculation failed (likely missing data from other semester)
            // This is expected and can be ignored
        }
    }

    public async Task<IEnumerable<StudentGradeViewDto>> GetStudentGradesViewAsync(string classId, string subjectId, string schoolYearId)
    {
        // Get all necessary data
        var classAssignments = await _classAssignmentRepository.GetStudentsByClassIdAsync(classId);
        var gradeTypes = await _gradeTypeRepository.GetAllAsync();
        var semesters = await _semesterRepository.GetAllAsync();

        var semester1 = semesters.FirstOrDefault(s => s.SemesterId == "HK1");
        var semester2 = semesters.FirstOrDefault(s => s.SemesterId == "HK2");

        // Get grades for both semesters
        var gradesHK1 = await _gradeRepository.GetGradesByClassSubjectSemesterAsync(classId, subjectId, "HK1", schoolYearId);
        var gradesHK2 = await _gradeRepository.GetGradesByClassSubjectSemesterAsync(classId, subjectId, "HK2", schoolYearId);

        var studentGradeViews = new List<StudentGradeViewDto>();

        foreach (var assignment in classAssignments)
        {
            var studentView = new StudentGradeViewDto
            {
                StudentId = assignment.StudentId,
                StudentName = assignment.Student.FullName
            };

            // Process Semester 1
            if (semester1 != null)
            {
                studentView.Semester1 = ProcessSemesterGrades(
                    assignment.StudentId,
                    gradesHK1,
                    gradeTypes,
                    semester1);
            }

            // Process Semester 2
            if (semester2 != null)
            {
                studentView.Semester2 = ProcessSemesterGrades(
                    assignment.StudentId,
                    gradesHK2,
                    gradeTypes,
                    semester2);
            }

            // Calculate year average based on subject type
            // Check if this is a Pass/Fail subject (no numeric averages)
            bool isPassFailSubject = studentView.Semester1?.Average == null &&
                                    studentView.Semester2?.Average == null &&
                                    (studentView.Semester1?.AverageDisplay != null ||
                                     studentView.Semester2?.AverageDisplay != null);

            if (isPassFailSubject)
            {
                // For Pass/Fail subjects, year result is based on Semester 2 only
                if (studentView.Semester2?.AverageDisplay != null)
                {
                    studentView.YearAverageDisplay = studentView.Semester2.AverageDisplay;
                }
            }
            else if (studentView.Semester1?.Average != null && studentView.Semester2?.Average != null &&
                     semester1 != null && semester2 != null)
            {
                // For numeric subjects, calculate weighted average
                var totalCoefficient = semester1.Coefficient + semester2.Coefficient;
                if (totalCoefficient > 0)
                {
                    studentView.YearAverage =
                        (studentView.Semester1.Average.Value * semester1.Coefficient +
                         studentView.Semester2.Average.Value * semester2.Coefficient) / totalCoefficient;
                    studentView.YearAverageDisplay = studentView.YearAverage.Value.ToString("F2");
                }
            }

            studentGradeViews.Add(studentView);
        }

        return studentGradeViews;
    }

    private SemesterGradesSummary ProcessSemesterGrades(
        string studentId,
        IEnumerable<Grade> semesterGrades,
        IEnumerable<GradeType> gradeTypes,
        Semester semester)
    {
        var summary = new SemesterGradesSummary
        {
            SemesterId = semester.SemesterId,
            Components = new List<GradeComponentDto>()
        };

        var studentGrades = semesterGrades.Where(g => g.StudentId == studentId).ToList();

        foreach (var gradeType in gradeTypes)
        {
            var grade = studentGrades.FirstOrDefault(g => g.GradeTypeId == gradeType.GradeTypeId);

            summary.Components.Add(new GradeComponentDto
            {
                GradeTypeId = gradeType.GradeTypeId,
                GradeTypeName = gradeType.GradeTypeName,
                Coefficient = gradeType.Coefficient,
                Score = grade?.Score,
                IsComment = grade?.IsComment ?? false,
                Comment = grade?.Comment
            });
        }

        // Calculate average or determine Pass/Fail
        var gradesWithScores = summary.Components.Where(c => !c.IsComment && c.Score != null).ToList();
        var gradesWithComments = summary.Components.Where(c => c.IsComment && !string.IsNullOrEmpty(c.Comment)).ToList();

        // Check if this is a Pass/Fail subject (all grades are comments)
        if (gradesWithComments.Any() && !gradesWithScores.Any())
        {
            // For Pass/Fail subjects, check if all are "Pass"
            bool allPass = gradesWithComments.All(c => c.Comment == "Pass");
            summary.AverageDisplay = allPass ? "Đạt" : "Chưa đạt";
        }
        else if (gradesWithScores.Any())
        {
            // For numeric subjects, calculate weighted average
            decimal totalWeightedScore = 0;
            int totalCoefficient = 0;

            foreach (var grade in gradesWithScores)
            {
                totalWeightedScore += grade.Score.Value * grade.Coefficient;
                totalCoefficient += grade.Coefficient;
            }

            if (totalCoefficient > 0)
            {
                summary.Average = totalWeightedScore / totalCoefficient;
                summary.AverageDisplay = summary.Average.Value.ToString("F2");
            }
        }

        return summary;
    }

    public async Task<object> ImportGradesFromExcelAsync(IFormFile file, string classId, string subjectId, string semesterId, string schoolYearId, bool isComment)
    {
        using var stream = file.OpenReadStream();
        using var package = new OfficeOpenXml.ExcelPackage(stream);

        var worksheet = package.Workbook.Worksheets[0]; // First sheet
        int rowCount = worksheet.Dimension?.Rows ?? 0;

        int successCount = 0;
        int errorCount = 0;
        var errors = new List<string>();

        // Start from row 2 (skip header row)
        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                // Expected columns: StudentId (A), GradeTypeId (B), Score/Comment (C)
                var studentId = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                var gradeTypeId = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                var valueCell = worksheet.Cells[row, 3].Value;

                if (string.IsNullOrEmpty(studentId) || string.IsNullOrEmpty(gradeTypeId))
                {
                    errors.Add($"Dòng {row}: Thiếu Mã HS hoặc Loại Điểm");
                    errorCount++;
                    continue;
                }

                // Validate Student exists
                var studentExists = await _context.Students.AnyAsync(s => s.StudentId == studentId);
                if (!studentExists)
                {
                    errors.Add($"Dòng {row}: Mã HS '{studentId}' không tồn tại trong hệ thống");
                    errorCount++;
                    continue;
                }

                // Validate GradeType exists
                var gradeTypeExists = await _context.GradeTypes.AnyAsync(gt => gt.GradeTypeId == gradeTypeId);
                if (!gradeTypeExists)
                {
                    errors.Add($"Dòng {row}: Loại Điểm '{gradeTypeId}' không tồn tại trong hệ thống");
                    errorCount++;
                    continue;
                }

                // Check if grade already exists
                var existingGrade = await _context.Grades
                    .FirstOrDefaultAsync(g => g.StudentId == studentId &&
                                            g.ClassId == classId &&
                                            g.SubjectId == subjectId &&
                                            g.SemesterId == semesterId &&
                                            g.SchoolYearId == schoolYearId &&
                                            g.GradeTypeId == gradeTypeId);

                if (isComment)
                {
                    // Handle comment import
                    var comment = valueCell?.ToString()?.Trim();
                    if (string.IsNullOrEmpty(comment) || (comment != "Pass" && comment != "Fail"))
                    {
                        errors.Add($"Dòng {row}: Nhận xét phải là 'Pass' hoặc 'Fail'");
                        errorCount++;
                        continue;
                    }

                    if (existingGrade != null)
                    {
                        // Update existing
                        existingGrade.IsComment = true;
                        existingGrade.Comment = comment;
                        existingGrade.Score = null;
                    }
                    else
                    {
                        // Create new
                        var newGrade = new Models.Grade
                        {
                            StudentId = studentId,
                            ClassId = classId,
                            SubjectId = subjectId,
                            SemesterId = semesterId,
                            SchoolYearId = schoolYearId,
                            GradeTypeId = gradeTypeId,
                            IsComment = true,
                            Comment = comment,
                            Score = null
                        };
                        _context.Grades.Add(newGrade);
                    }
                }
                else
                {
                    // Handle score import
                    if (valueCell == null || !decimal.TryParse(valueCell.ToString(), out decimal score))
                    {
                        errors.Add($"Dòng {row}: Điểm không hợp lệ");
                        errorCount++;
                        continue;
                    }

                    if (score < 0 || score > 10)
                    {
                        errors.Add($"Dòng {row}: Điểm phải từ 0 đến 10");
                        errorCount++;
                        continue;
                    }

                    if (existingGrade != null)
                    {
                        // Update existing
                        existingGrade.IsComment = false;
                        existingGrade.Score = score;
                        existingGrade.Comment = null;
                    }
                    else
                    {
                        // Create new
                        var newGrade = new Models.Grade
                        {
                            StudentId = studentId,
                            ClassId = classId,
                            SubjectId = subjectId,
                            SemesterId = semesterId,
                            SchoolYearId = schoolYearId,
                            GradeTypeId = gradeTypeId,
                            IsComment = false,
                            Score = score,
                            Comment = null
                        };
                        _context.Grades.Add(newGrade);
                    }
                }

                successCount++;
            }
            catch (Exception ex)
            {
                errors.Add($"Dòng {row}: {ex.Message}");
                errorCount++;
            }
        }

        // Save all changes
        await _context.SaveChangesAsync();

        return new
        {
            successCount,
            errorCount,
            errors,
            message = $"Import thành công {successCount} điểm, {errorCount} lỗi"
        };
    }

    public async Task<byte[]> ExportGradesAsync(string classId, string subjectId, string semesterId, string schoolYearId, string format)
    {
        // Get data for export
        var classInfo = await _context.Classes.FindAsync(classId);
        var subjectInfo = await _context.Subjects.FindAsync(subjectId);
        var semesterInfo = await _context.Semesters.FindAsync(semesterId);
        var schoolYearInfo = await _context.SchoolYears.FindAsync(schoolYearId);

        var studentGrades = await GetStudentGradesForClassSubjectAsync(classId, subjectId, semesterId, schoolYearId);
        var gradeTypes = await _gradeTypeRepository.GetAllAsync();
        var sortedGradeTypes = gradeTypes.OrderBy(gt => gt.Coefficient).ToList();

        return format.ToLower() switch
        {
            "excel" => await ExportToExcelAsync(classInfo, subjectInfo, semesterInfo, schoolYearInfo, studentGrades, sortedGradeTypes),
            "word" => await ExportToWordAsync(classInfo, subjectInfo, semesterInfo, schoolYearInfo, studentGrades, sortedGradeTypes),
            "pdf" => await ExportToPdfAsync(classInfo, subjectInfo, semesterInfo, schoolYearInfo, studentGrades, sortedGradeTypes),
            _ => throw new ArgumentException("Unsupported format")
        };
    }

    private async Task<byte[]> ExportToExcelAsync(
        Class? classInfo,
        Subject? subjectInfo,
        Semester? semesterInfo,
        SchoolYear? schoolYearInfo,
        IEnumerable<StudentGradeDto> studentGrades,
        List<GradeType> sortedGradeTypes)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Bảng Điểm");

        // Header information
        worksheet.Cells["A1"].Value = $"BẢNG ĐIỂM HỌC SINH";
        worksheet.Cells["A1"].Style.Font.Bold = true;
        worksheet.Cells["A1"].Style.Font.Size = 16;
        worksheet.Cells["A1:H1"].Merge = true;
        worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        worksheet.Cells["A2"].Value = $"Lớp: {classInfo?.ClassName} - Môn: {subjectInfo?.SubjectName}";
        worksheet.Cells["A2:H2"].Merge = true;
        worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        worksheet.Cells["A3"].Value = $"Học kỳ: {semesterInfo?.SemesterName} - Năm học: {schoolYearInfo?.SchoolYearName}";
        worksheet.Cells["A3:H3"].Merge = true;
        worksheet.Cells["A3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        // Table headers
        int row = 5;
        int col = 1;
        worksheet.Cells[row, col++].Value = "STT";
        worksheet.Cells[row, col++].Value = "Mã HS";
        worksheet.Cells[row, col++].Value = "Họ và Tên";

        foreach (var gradeType in sortedGradeTypes)
        {
            worksheet.Cells[row, col++].Value = gradeType.GradeTypeName;
        }
        worksheet.Cells[row, col].Value = "Điểm TB";

        // Style headers
        using (var range = worksheet.Cells[row, 1, row, col])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
        }

        // Data rows
        row++;
        int stt = 1;
        foreach (var student in studentGrades)
        {
            col = 1;
            worksheet.Cells[row, col++].Value = stt++;
            worksheet.Cells[row, col++].Value = student.StudentId;
            worksheet.Cells[row, col++].Value = student.StudentName;

            foreach (var gradeType in sortedGradeTypes)
            {
                var grade = student.Grades.FirstOrDefault(g => g.GradeTypeId == gradeType.GradeTypeId);
                if (grade?.IsComment == true)
                {
                    worksheet.Cells[row, col++].Value = grade.Comment ?? "-";
                }
                else
                {
                    worksheet.Cells[row, col++].Value = grade?.Score?.ToString() ?? "-";
                }
            }

            // Calculate average
            var average = CalculateStudentAverage(student);
            worksheet.Cells[row, col].Value = average;
            row++;
        }

        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();

        return await Task.FromResult(package.GetAsByteArray());
    }

    private async Task<byte[]> ExportToWordAsync(
        Class? classInfo,
        Subject? subjectInfo,
        Semester? semesterInfo,
        SchoolYear? schoolYearInfo,
        IEnumerable<StudentGradeDto> studentGrades,
        List<GradeType> sortedGradeTypes)
    {
        using var document = DocX.Create(new MemoryStream());

        // Title
        var title = document.InsertParagraph($"BẢNG ĐIỂM HỌC SINH")
            .FontSize(16)
            .Bold()
            .Alignment = Alignment.center;

        document.InsertParagraph($"Lớp: {classInfo?.ClassName} - Môn: {subjectInfo?.SubjectName}")
            .FontSize(12)
            .Alignment = Alignment.center;

        document.InsertParagraph($"Học kỳ: {semesterInfo?.SemesterName} - Năm học: {schoolYearInfo?.SchoolYearName}")
            .FontSize(12)
            .Alignment = Alignment.center;

        document.InsertParagraph(); // Empty line

        // Create table
        int columnCount = 3 + sortedGradeTypes.Count + 1; // STT + Mã HS + Họ tên + grades + average
        var table = document.AddTable(studentGrades.Count() + 1, columnCount);
        table.Design = TableDesign.LightGridAccent1;

        // Headers
        int col = 0;
        table.Rows[0].Cells[col++].Paragraphs[0].Append("STT").Bold();
        table.Rows[0].Cells[col++].Paragraphs[0].Append("Mã HS").Bold();
        table.Rows[0].Cells[col++].Paragraphs[0].Append("Họ và Tên").Bold();

        foreach (var gradeType in sortedGradeTypes)
        {
            table.Rows[0].Cells[col++].Paragraphs[0].Append(gradeType.GradeTypeName).Bold();
        }
        table.Rows[0].Cells[col].Paragraphs[0].Append("Điểm TB").Bold();

        // Data rows
        int rowIndex = 1;
        int stt = 1;
        foreach (var student in studentGrades)
        {
            col = 0;
            table.Rows[rowIndex].Cells[col++].Paragraphs[0].Append(stt++.ToString());
            table.Rows[rowIndex].Cells[col++].Paragraphs[0].Append(student.StudentId);
            table.Rows[rowIndex].Cells[col++].Paragraphs[0].Append(student.StudentName);

            foreach (var gradeType in sortedGradeTypes)
            {
                var grade = student.Grades.FirstOrDefault(g => g.GradeTypeId == gradeType.GradeTypeId);
                string gradeValue = "-";
                if (grade?.IsComment == true)
                {
                    gradeValue = grade.Comment ?? "-";
                }
                else if (grade?.Score != null)
                {
                    gradeValue = grade.Score.ToString();
                }
                table.Rows[rowIndex].Cells[col++].Paragraphs[0].Append(gradeValue);
            }

            var average = CalculateStudentAverage(student);
            table.Rows[rowIndex].Cells[col].Paragraphs[0].Append(average);
            rowIndex++;
        }

        document.InsertTable(table);

        using var stream = new MemoryStream();
        document.SaveAs(stream);
        return await Task.FromResult(stream.ToArray());
    }

    private async Task<byte[]> ExportToPdfAsync(
        Class? classInfo,
        Subject? subjectInfo,
        Semester? semesterInfo,
        SchoolYear? schoolYearInfo,
        IEnumerable<StudentGradeDto> studentGrades,
        List<GradeType> sortedGradeTypes)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(column =>
                {
                    column.Item().AlignCenter().Text("BẢNG ĐIỂM HỌC SINH").FontSize(16).Bold();
                    column.Item().AlignCenter().Text($"Lớp: {classInfo?.ClassName} - Môn: {subjectInfo?.SubjectName}").FontSize(12);
                    column.Item().AlignCenter().Text($"Học kỳ: {semesterInfo?.SemesterName} - Năm học: {schoolYearInfo?.SchoolYearName}").FontSize(12);
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                {
                    // Define columns
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30); // STT
                        columns.ConstantColumn(70); // Mã HS
                        columns.RelativeColumn(2); // Họ và Tên
                        foreach (var _ in sortedGradeTypes)
                        {
                            columns.ConstantColumn(60); // Grade columns
                        }
                        columns.ConstantColumn(60); // Điểm TB
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("STT").Bold();
                        header.Cell().Element(CellStyle).Text("Mã HS").Bold();
                        header.Cell().Element(CellStyle).Text("Họ và Tên").Bold();

                        foreach (var gradeType in sortedGradeTypes)
                        {
                            header.Cell().Element(CellStyle).Text(gradeType.GradeTypeName).Bold();
                        }
                        header.Cell().Element(CellStyle).Text("Điểm TB").Bold();

                        static IContainer CellStyle(IContainer container)
                        {
                            return container
                                .Border(1)
                                .Background(Colors.Grey.Lighten3)
                                .Padding(5)
                                .AlignCenter()
                                .AlignMiddle();
                        }
                    });

                    // Data rows
                    int stt = 1;
                    foreach (var student in studentGrades)
                    {
                        table.Cell().Element(CellStyle).Text(stt++.ToString());
                        table.Cell().Element(CellStyle).Text(student.StudentId);
                        table.Cell().Element(CellStyle).Text(student.StudentName);

                        foreach (var gradeType in sortedGradeTypes)
                        {
                            var grade = student.Grades.FirstOrDefault(g => g.GradeTypeId == gradeType.GradeTypeId);
                            string gradeValue = "-";
                            if (grade?.IsComment == true)
                            {
                                gradeValue = grade.Comment ?? "-";
                            }
                            else if (grade?.Score != null)
                            {
                                gradeValue = grade.Score.ToString();
                            }
                            table.Cell().Element(CellStyle).Text(gradeValue);
                        }

                        var average = CalculateStudentAverage(student);
                        table.Cell().Element(CellStyle).Text(average);

                        static IContainer CellStyle(IContainer container)
                        {
                            return container
                                .Border(1)
                                .Padding(5)
                                .AlignCenter()
                                .AlignMiddle();
                        }
                    }
                });
            });
        });

        return await Task.FromResult(document.GeneratePdf());
    }

    private string CalculateStudentAverage(StudentGradeDto student)
    {
        var grades = student.Grades.Where(g => g.RowNumber > 0).ToList();

        if (grades.Count == 0)
        {
            return "-";
        }

        // Check if all grades are in comment mode
        var allComments = grades.All(g => g.IsComment);

        if (allComments)
        {
            var allPass = grades.All(g => g.Comment == "Pass");
            return allPass ? "Đạt" : "Chưa đạt";
        }

        // Check if all grades are in score mode
        var allScores = grades.All(g => !g.IsComment && g.Score != null);

        if (allScores)
        {
            decimal totalWeightedScore = 0;
            int totalCoefficient = 0;

            foreach (var grade in grades)
            {
                if (grade.Score != null)
                {
                    totalWeightedScore += grade.Score.Value * grade.Coefficient;
                    totalCoefficient += grade.Coefficient;
                }
            }

            if (totalCoefficient == 0)
            {
                return "-";
            }

            var average = totalWeightedScore / totalCoefficient;
            return average.ToString("F2");
        }

        return "-";
    }
}
