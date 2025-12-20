import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface GradeLevelResultsDto {
  gradeLevelId: string;
  gradeLevelName: string;
  classes: ClassResultsDto[];
}

export interface ClassResultsDto {
  classId: string;
  className: string;
  students: StudentResultDto[];
}

export interface StudentResultDto {
  studentId: string;
  studentName: string;

  // Semester 1
  semester1AcademicPerformance?: string;
  semester1Conduct?: string;
  semester1Average?: number;

  // Semester 2
  semester2AcademicPerformance?: string;
  semester2Conduct?: string;
  semester2Average?: number;

  // Year
  yearAcademicPerformance?: string;
  yearConduct?: string;
  yearResult?: string;
  yearAverage?: number;

  // Subject details
  subjectResults: SubjectResultDto[];
}

export interface SubjectResultDto {
  subjectId: string;
  subjectName: string;
  semester1Average?: number;
  semester2Average?: number;
}

export interface SubjectDetailedGradesDto {
  studentId: string;
  studentName: string;
  subjectId: string;
  subjectName: string;
  semesters: SemesterDetailedGradesDto[];
}

export interface SemesterDetailedGradesDto {
  semesterId: string;
  semesterName: string;
  average?: number;
  grades: GradeEntryDto[];
}

export interface GradeEntryDto {
  gradeTypeId: string;
  gradeTypeName: string;
  coefficient: number;
  score?: number;
  isComment: boolean;
  comment?: string;
}

@Injectable({
  providedIn: 'root'
})
export class PrincipalDashboardService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/PrincipalDashboard`;

  /**
   * Get all grade level results for a school year
   */
  getGradeLevelResults(schoolYearId: string): Observable<GradeLevelResultsDto[]> {
    return this.http.get<GradeLevelResultsDto[]>(
      `${this.apiUrl}/grade-results/${schoolYearId}`
    );
  }

  /**
   * Get detailed grades for a specific student and subject
   */
  getSubjectDetailedGrades(
    studentId: string,
    classId: string,
    schoolYearId: string,
    subjectId: string
  ): Observable<SubjectDetailedGradesDto> {
    return this.http.get<SubjectDetailedGradesDto>(
      `${this.apiUrl}/subject-details/${studentId}/${classId}/${schoolYearId}/${subjectId}`
    );
  }

  /**
   * Recalculate all rankings for all students in a school year
   */
  recalculateAllRankings(schoolYearId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/recalculate-all/${schoolYearId}`, null);
  }
}
