import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { SchoolYearDto, CreateSchoolYearDto, UpdateSchoolYearDto } from '../models/schoolyear.model';

@Injectable({
  providedIn: 'root'
})
export class SchoolYearService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/SchoolYear`;

  getAllSchoolYears(): Observable<SchoolYearDto[]> {
    return this.http.get<SchoolYearDto[]>(this.apiUrl);
  }

  getSchoolYearById(schoolYearId: string): Observable<SchoolYearDto> {
    return this.http.get<SchoolYearDto>(`${this.apiUrl}/${schoolYearId}`);
  }

  createSchoolYear(createDto: CreateSchoolYearDto): Observable<SchoolYearDto> {
    return this.http.post<SchoolYearDto>(this.apiUrl, createDto);
  }

  updateSchoolYear(schoolYearId: string, updateDto: UpdateSchoolYearDto): Observable<SchoolYearDto> {
    return this.http.put<SchoolYearDto>(`${this.apiUrl}/${schoolYearId}`, updateDto);
  }

  deleteSchoolYear(schoolYearId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${schoolYearId}`);
  }
}
