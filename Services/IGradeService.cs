using API.DTOs;
using Microsoft.AspNetCore.Http;

namespace API.Services;

public interface IGradeService
{
    Task<IEnumerable<TeacherClassSubjectDto>> GetTeacherClassSubjectsAsync(string teacherId);
    Task<IEnumerable<StudentGradeDto>> GetStudentGradesForClassSubjectAsync(string classId, string subjectId, string semesterId, string schoolYearId);
    Task<GradeDto> CreateGradeAsync(CreateGradeDto createGradeDto);
    Task<GradeDto> UpdateGradeAsync(int rowNumber, UpdateGradeDto updateGradeDto);
    Task DeleteGradeAsync(int rowNumber);
    Task<IEnumerable<StudentGradeViewDto>> GetStudentGradesViewAsync(string classId, string subjectId, string schoolYearId);
    Task<object> ImportGradesFromExcelAsync(IFormFile file, string classId, string subjectId, string semesterId, string schoolYearId, bool isComment);
    Task<byte[]> ExportGradesAsync(string classId, string subjectId, string semesterId, string schoolYearId, string format);
}
