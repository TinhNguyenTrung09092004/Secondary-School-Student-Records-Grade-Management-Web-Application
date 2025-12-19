namespace API.DTOs;

public class GradeDto
{
    public int RowNumber { get; set; }
    public string StudentId { get; set; } = null!;
    public string SubjectId { get; set; } = null!;
    public string SemesterId { get; set; } = null!;
    public string SchoolYearId { get; set; } = null!;
    public string ClassId { get; set; } = null!;
    public string GradeTypeId { get; set; } = null!;
    public decimal? Score { get; set; }
    public bool IsComment { get; set; }
    public string? Comment { get; set; }
}

public class CreateGradeDto
{
    public string StudentId { get; set; } = null!;
    public string SubjectId { get; set; } = null!;
    public string SemesterId { get; set; } = null!;
    public string SchoolYearId { get; set; } = null!;
    public string ClassId { get; set; } = null!;
    public string GradeTypeId { get; set; } = null!;
    public decimal? Score { get; set; }
    public bool IsComment { get; set; }
    public string? Comment { get; set; }
}

public class UpdateGradeDto
{
    public decimal? Score { get; set; }
    public bool IsComment { get; set; }
    public string? Comment { get; set; }
}

public class StudentGradeDto
{
    public string StudentId { get; set; } = null!;
    public string StudentName { get; set; } = null!;
    public List<GradeDetailDto> Grades { get; set; } = new List<GradeDetailDto>();
}

public class GradeDetailDto
{
    public int RowNumber { get; set; }
    public string GradeTypeId { get; set; } = null!;
    public string GradeTypeName { get; set; } = null!;
    public int Coefficient { get; set; }
    public decimal? Score { get; set; }
    public bool IsComment { get; set; }
    public string? Comment { get; set; }
}

public class TeacherClassSubjectDto
{
    public string ClassId { get; set; } = null!;
    public string ClassName { get; set; } = null!;
    public string SubjectId { get; set; } = null!;
    public string SubjectName { get; set; } = null!;
    public string SchoolYearId { get; set; } = null!;
    public string SchoolYearName { get; set; } = null!;
}

// DTOs for Grade Viewing Feature
public class StudentGradeViewDto
{
    public string StudentId { get; set; } = null!;
    public string StudentName { get; set; } = null!;
    public SemesterGradesSummary? Semester1 { get; set; }
    public SemesterGradesSummary? Semester2 { get; set; }
    public decimal? YearAverage { get; set; }
    public string? YearAverageDisplay { get; set; }
}

public class SemesterGradesSummary
{
    public string SemesterId { get; set; } = null!;
    public decimal? Average { get; set; }
    public string? AverageDisplay { get; set; }
    public List<GradeComponentDto> Components { get; set; } = new List<GradeComponentDto>();
}

public class GradeComponentDto
{
    public string GradeTypeId { get; set; } = null!;
    public string GradeTypeName { get; set; } = null!;
    public int Coefficient { get; set; }
    public decimal? Score { get; set; }
    public bool IsComment { get; set; }
    public string? Comment { get; set; }
}
