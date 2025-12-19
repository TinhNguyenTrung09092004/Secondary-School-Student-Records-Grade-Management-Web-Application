import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { TeacherDto, CreateTeacherProfileDto, UpdateTeacherProfileDto } from '../models/teacher.model';

@Injectable({
  providedIn: 'root'
})
export class TeacherService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Teacher`;

  getAllTeachers(): Observable<TeacherDto[]> {
    return this.http.get<TeacherDto[]>(this.apiUrl);
  }

  getProfile(): Observable<TeacherDto> {
    return this.http.get<TeacherDto>(`${this.apiUrl}/profile`);
  }

  createProfile(createDto: CreateTeacherProfileDto): Observable<TeacherDto> {
    return this.http.post<TeacherDto>(`${this.apiUrl}/profile`, createDto);
  }

  updateProfile(updateDto: UpdateTeacherProfileDto): Observable<TeacherDto> {
    return this.http.put<TeacherDto>(`${this.apiUrl}/profile`, updateDto);
  }

  deleteProfile(): Observable<any> {
    return this.http.delete(`${this.apiUrl}/profile`);
  }
}
