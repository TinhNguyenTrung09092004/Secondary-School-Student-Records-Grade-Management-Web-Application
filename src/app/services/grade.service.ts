import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  GradeDto,
  CreateGradeDto,
  UpdateGradeDto,
  StudentGradeDto,
  TeacherClassSubjectDto,
  SemesterDto,
  GradeTypeDto,
  CreateGradeTypeDto,
  UpdateGradeTypeDto,
  StudentGradeViewDto
} from '../models/grade.model';

@Injectable({
  providedIn: 'root'
})
export class GradeService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Grade`;

  getTeacherClasses(): Observable<TeacherClassSubjectDto[]> {
    return this.http.get<TeacherClassSubjectDto[]>(`${this.apiUrl}/teacher-classes`);
  }

  getStudentGrades(
    classId: string,
    subjectId: string,
    semesterId: string,
    schoolYearId: string
  ): Observable<StudentGradeDto[]> {
    const params = new HttpParams()
      .set('classId', classId)
      .set('subjectId', subjectId)
      .set('semesterId', semesterId)
      .set('schoolYearId', schoolYearId);

    return this.http.get<StudentGradeDto[]>(`${this.apiUrl}/students`, { params });
  }

  createGrade(createGradeDto: CreateGradeDto): Observable<GradeDto> {
    return this.http.post<GradeDto>(this.apiUrl, createGradeDto);
  }

  updateGrade(rowNumber: number, updateGradeDto: UpdateGradeDto): Observable<GradeDto> {
    return this.http.put<GradeDto>(`${this.apiUrl}/${rowNumber}`, updateGradeDto);
  }

  deleteGrade(rowNumber: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${rowNumber}`);
  }

  getSemesters(): Observable<SemesterDto[]> {
    return this.http.get<SemesterDto[]>(`${environment.apiUrl}/api/Semester`);
  }

  getGradeTypes(): Observable<GradeTypeDto[]> {
    return this.http.get<GradeTypeDto[]>(`${environment.apiUrl}/api/GradeType`);
  }

  createGradeType(createGradeTypeDto: CreateGradeTypeDto): Observable<GradeTypeDto> {
    return this.http.post<GradeTypeDto>(`${environment.apiUrl}/api/GradeType`, createGradeTypeDto);
  }

  updateGradeType(id: string, updateGradeTypeDto: UpdateGradeTypeDto): Observable<GradeTypeDto> {
    return this.http.put<GradeTypeDto>(`${environment.apiUrl}/api/GradeType/${id}`, updateGradeTypeDto);
  }

  deleteGradeType(id: string): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/api/GradeType/${id}`);
  }

  getStudentGradesView(
    classId: string,
    subjectId: string,
    schoolYearId: string
  ): Observable<StudentGradeViewDto[]> {
    const params = new HttpParams()
      .set('classId', classId)
      .set('subjectId', subjectId)
      .set('schoolYearId', schoolYearId);

    return this.http.get<StudentGradeViewDto[]>(`${this.apiUrl}/view`, { params });
  }

  importGradesFromExcel(formData: FormData): Observable<any> {
    return this.http.post(`${this.apiUrl}/import-excel`, formData);
  }

  exportGrades(
    classId: string,
    subjectId: string,
    semesterId: string,
    schoolYearId: string,
    format: string
  ): Observable<Blob> {
    const params = new HttpParams()
      .set('classId', classId)
      .set('subjectId', subjectId)
      .set('semesterId', semesterId)
      .set('schoolYearId', schoolYearId)
      .set('format', format);

    return this.http.get(`${this.apiUrl}/export`, {
      params,
      responseType: 'blob'
    });
  }
}
