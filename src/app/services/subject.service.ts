import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { SubjectDto, CreateSubjectDto, UpdateSubjectDto } from '../models/subject.model';

@Injectable({
  providedIn: 'root'
})
export class SubjectService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/subject`;

  getAllSubjects(): Observable<SubjectDto[]> {
    return this.http.get<SubjectDto[]>(this.apiUrl);
  }

  getSubjectById(subjectId: string): Observable<SubjectDto> {
    return this.http.get<SubjectDto>(`${this.apiUrl}/${subjectId}`);
  }

  createSubject(createDto: CreateSubjectDto): Observable<SubjectDto> {
    return this.http.post<SubjectDto>(this.apiUrl, createDto);
  }

  updateSubject(subjectId: string, updateDto: UpdateSubjectDto): Observable<SubjectDto> {
    return this.http.put<SubjectDto>(`${this.apiUrl}/${subjectId}`, updateDto);
  }

  deleteSubject(subjectId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${subjectId}`);
  }
}
