import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  TeachingAssignmentDto,
  CreateTeachingAssignmentDto,
  UpdateTeachingAssignmentDto,
  DepartmentTeacherDto,
  DepartmentHeadCheckDto
} from '../models/teachingassignment.model';

@Injectable({
  providedIn: 'root'
})
export class TeachingAssignmentService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/TeachingAssignment`;

  checkIsDepartmentHead(): Observable<DepartmentHeadCheckDto> {
    return this.http.get<DepartmentHeadCheckDto>(`${this.apiUrl}/department-head/check`);
  }

  getDepartmentTeachers(): Observable<DepartmentTeacherDto[]> {
    return this.http.get<DepartmentTeacherDto[]>(`${this.apiUrl}/department-head/teachers`);
  }

  getAllTeachingAssignments(): Observable<TeachingAssignmentDto[]> {
    return this.http.get<TeachingAssignmentDto[]>(this.apiUrl);
  }

  createTeachingAssignment(createDto: CreateTeachingAssignmentDto): Observable<TeachingAssignmentDto> {
    return this.http.post<TeachingAssignmentDto>(`${this.apiUrl}/department-head/assign`, createDto);
  }

  updateTeachingAssignment(id: number, updateDto: UpdateTeachingAssignmentDto): Observable<TeachingAssignmentDto> {
    return this.http.put<TeachingAssignmentDto>(`${this.apiUrl}/department-head/${id}`, updateDto);
  }

  deleteTeachingAssignment(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/department-head/${id}`);
  }
}
