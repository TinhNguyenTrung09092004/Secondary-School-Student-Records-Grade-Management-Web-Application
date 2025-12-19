import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface AcademicRankingPreview {
  academicPerformanceId?: string;
  resultId?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AcademicRankingService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/AcademicRanking`;

  /**
   * Calculate and update semester academic performance for a student
   */
  calculateSemesterAcademicPerformance(
    studentId: string,
    classId: string,
    schoolYearId: string,
    semesterId: string
  ): Observable<any> {
    return this.http.post(`${this.apiUrl}/semester/calculate`, null, {
      params: { studentId, classId, schoolYearId, semesterId }
    });
  }

  /**
   * Calculate and update year academic performance and result title for a student
   */
  calculateYearAcademicPerformance(
    studentId: string,
    classId: string,
    schoolYearId: string
  ): Observable<any> {
    return this.http.post(`${this.apiUrl}/year/calculate`, null, {
      params: { studentId, classId, schoolYearId }
    });
  }

  /**
   * Preview semester academic performance without saving
   */
  previewSemesterAcademicPerformance(
    studentId: string,
    classId: string,
    schoolYearId: string,
    semesterId: string
  ): Observable<{ academicPerformanceId: string }> {
    return this.http.get<{ academicPerformanceId: string }>(
      `${this.apiUrl}/semester/preview`,
      { params: { studentId, classId, schoolYearId, semesterId } }
    );
  }

  /**
   * Preview year academic performance and result title without saving
   */
  previewYearAcademicPerformance(
    studentId: string,
    classId: string,
    schoolYearId: string
  ): Observable<AcademicRankingPreview> {
    return this.http.get<AcademicRankingPreview>(
      `${this.apiUrl}/year/preview`,
      { params: { studentId, classId, schoolYearId } }
    );
  }
}