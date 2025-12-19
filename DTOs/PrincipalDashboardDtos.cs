namespace API.DTOs;

public class GradeLevelResultsDto
{
    public string GradeLevelId { get; set; } = null!;
    public string GradeLevelName { get; set; } = null!;
    public List<ClassResultsDto> Classes { get; set; } = new();
}

public class ClassResultsDto
{
    public string ClassId { get; set; } = null!;
    public string ClassName { get; set; } = null!;
    public List<StudentResultDto> Students { get; set; } = new();
}

public class StudentResultDto
{
    public string StudentId { get; set; } = null!;
    public string StudentName { get; set; } = null!;

    // Semester 1
    public string? Semester1AcademicPerformance { get; set; }
    public string? Semester1Conduct { get; set; }
    public decimal? Semester1Average { get; set; }

    // Semester 2
    public string? Semester2AcademicPerformance { get; set; }
    public string? Semester2Conduct { get; set; }
    public decimal? Semester2Average { get; set; }

    // Year
    public string? YearAcademicPerformance { get; set; }
    public string? YearConduct { get; set; }
    public string? YearResult { get; set; }
    public decimal? YearAverage { get; set; }

    // Subject details
    public List<SubjectResultDto> SubjectResults { get; set; } = new();
}

public class SubjectResultDto
{
    public string SubjectId { get; set; } = null!;
    public string SubjectName { get; set; } = null!;
    public decimal? Semester1Average { get; set; }
    public decimal? Semester2Average { get; set; }
    public decimal? YearAverage { get; set; }
}
