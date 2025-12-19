import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { AcademicPerformanceDto, CreateAcademicPerformanceDto, UpdateAcademicPerformanceDto } from '../models/academic-performance.model';

@Injectable({
  providedIn: 'root'
})
export class AcademicPerformanceService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/AcademicPerformance`;

  getAll(): Observable<AcademicPerformanceDto[]> {
    return this.http.get<AcademicPerformanceDto[]>(this.apiUrl);
  }

  getById(id: string): Observable<AcademicPerformanceDto> {
    return this.http.get<AcademicPerformanceDto>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateAcademicPerformanceDto): Observable<AcademicPerformanceDto> {
    return this.http.post<AcademicPerformanceDto>(this.apiUrl, dto);
  }

  update(id: string, dto: UpdateAcademicPerformanceDto): Observable<AcademicPerformanceDto> {
    return this.http.put<AcademicPerformanceDto>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
