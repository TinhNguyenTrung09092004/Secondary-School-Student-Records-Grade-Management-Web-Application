using API.DTOs;
using API.Models;
using API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class GradeService : IGradeService
{
    private readonly IGradeRepository _gradeRepository;
    private readonly ITeachingAssignmentRepository _teachingAssignmentRepository;
    private readonly IClassAssignmentRepository _classAssignmentRepository;
    private readonly IGradeTypeRepository _gradeTypeRepository;
    private readonly ISemesterRepository _semesterRepository;

    public GradeService(
        IGradeRepository gradeRepository,
        ITeachingAssignmentRepository teachingAssignmentRepository,
        IClassAssignmentRepository classAssignmentRepository,
        IGradeTypeRepository gradeTypeRepository,
        ISemesterRepository semesterRepository)
    {
        _gradeRepository = gradeRepository;
        _teachingAssignmentRepository = teachingAssignmentRepository;
        _classAssignmentRepository = classAssignmentRepository;
        _gradeTypeRepository = gradeTypeRepository;
        _semesterRepository = semesterRepository;
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

        await _gradeRepository.DeleteAsync(rowNumber);
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
}
