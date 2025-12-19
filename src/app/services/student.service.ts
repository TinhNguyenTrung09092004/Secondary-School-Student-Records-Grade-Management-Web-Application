import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { StudentDto, CreateStudentDto, UpdateStudentDto } from '../models/hocsinh.model';

@Injectable({
  providedIn: 'root'
})
export class StudentService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/student`;

  getAllStudents(): Observable<StudentDto[]> {
    return this.http.get<StudentDto[]>(this.apiUrl);
  }

  getStudentById(studentId: string): Observable<StudentDto> {
    return this.http.get<StudentDto>(`${this.apiUrl}/${studentId}`);
  }

  createStudent(createDto: CreateStudentDto): Observable<StudentDto> {
    return this.http.post<StudentDto>(this.apiUrl, createDto);
  }

  updateStudent(studentId: string, updateDto: UpdateStudentDto): Observable<StudentDto> {
    return this.http.put<StudentDto>(`${this.apiUrl}/${studentId}`, updateDto);
  }

  deleteStudent(studentId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${studentId}`);
  }
}
