import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  ClassInfoDto,
  StudentInClassDto,
  AssignStudentToClassDto,
  BulkAssignStudentsDto,
  RemoveStudentFromClassDto,
  ClassAssignmentDto
} from '../models/classassignment.model';

@Injectable({
  providedIn: 'root'
})
export class ClassAssignmentService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/ClassAssignment`;

  getClassInfo(schoolYearId: string, gradeLevelId: string, classId: string): Observable<ClassInfoDto> {
    const params = new HttpParams()
      .set('schoolYearId', schoolYearId)
      .set('gradeLevelId', gradeLevelId)
      .set('classId', classId);
    return this.http.get<ClassInfoDto>(`${this.apiUrl}/class-info`, { params });
  }

  getStudentsInClass(schoolYearId: string, gradeLevelId: string, classId: string): Observable<StudentInClassDto[]> {
    const params = new HttpParams()
      .set('schoolYearId', schoolYearId)
      .set('gradeLevelId', gradeLevelId)
      .set('classId', classId);
    return this.http.get<StudentInClassDto[]>(`${this.apiUrl}/students-in-class`, { params });
  }

  getAvailableStudents(schoolYearId: string, gradeLevelId: string): Observable<StudentInClassDto[]> {
    const params = new HttpParams()
      .set('schoolYearId', schoolYearId)
      .set('gradeLevelId', gradeLevelId);
    return this.http.get<StudentInClassDto[]>(`${this.apiUrl}/available-students`, { params });
  }

  assignStudentToClass(assignDto: AssignStudentToClassDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/assign`, assignDto);
  }

  bulkAssignStudents(bulkAssignDto: BulkAssignStudentsDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/bulk-assign`, bulkAssignDto);
  }

  removeStudentFromClass(removeDto: RemoveStudentFromClassDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/remove`, removeDto);
  }

  getStudentClassHistory(studentId: string): Observable<ClassAssignmentDto[]> {
    return this.http.get<ClassAssignmentDto[]>(`${this.apiUrl}/student-history/${studentId}`);
  }
}
