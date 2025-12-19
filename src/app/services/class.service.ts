import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ClassDto, CreateClassDto, UpdateClassDto } from '../models/class.model';
import { TeacherDto } from '../models/teacher.model';

@Injectable({
  providedIn: 'root'
})
export class ClassService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Class`;

  getAllClasses(): Observable<ClassDto[]> {
    return this.http.get<ClassDto[]>(this.apiUrl);
  }

  getClassById(classId: string): Observable<ClassDto> {
    return this.http.get<ClassDto>(`${this.apiUrl}/${classId}`);
  }

  getClassesByYearAndGrade(schoolYearId: string, gradeLevelId: string): Observable<ClassDto[]> {
    const params = new HttpParams()
      .set('schoolYearId', schoolYearId)
      .set('gradeLevelId', gradeLevelId);
    return this.http.get<ClassDto[]>(`${this.apiUrl}/by-year-grade`, { params });
  }

  createClass(createDto: CreateClassDto): Observable<ClassDto> {
    return this.http.post<ClassDto>(this.apiUrl, createDto);
  }

  updateClass(classId: string, updateDto: UpdateClassDto): Observable<ClassDto> {
    return this.http.put<ClassDto>(`${this.apiUrl}/${classId}`, updateDto);
  }

  getEligibleHomeroomTeachers(classId: string): Observable<TeacherDto[]> {
    return this.http.get<TeacherDto[]>(`${this.apiUrl}/${classId}/eligible-homeroom-teachers`);
  }

  assignHomeroomTeacher(classId: string, teacherId: string): Observable<ClassDto> {
    return this.http.patch<ClassDto>(`${this.apiUrl}/${classId}/assign-teacher`, { teacherId });
  }

  deleteClass(classId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${classId}`);
  }
}
