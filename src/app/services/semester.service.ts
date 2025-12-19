import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { SemesterDto, CreateSemesterDto, UpdateSemesterDto } from '../models/semester.model';

@Injectable({
  providedIn: 'root'
})
export class SemesterService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Semester`;

  getAllSemesters(): Observable<SemesterDto[]> {
    return this.http.get<SemesterDto[]>(this.apiUrl);
  }

  getSemesterById(id: string): Observable<SemesterDto> {
    return this.http.get<SemesterDto>(`${this.apiUrl}/${id}`);
  }

  createSemester(createDto: CreateSemesterDto): Observable<SemesterDto> {
    return this.http.post<SemesterDto>(this.apiUrl, createDto);
  }

  updateSemester(id: string, updateDto: UpdateSemesterDto): Observable<SemesterDto> {
    return this.http.put<SemesterDto>(`${this.apiUrl}/${id}`, updateDto);
  }

  deleteSemester(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
