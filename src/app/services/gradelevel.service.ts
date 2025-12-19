import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { GradeLevelDto, CreateGradeLevelDto, UpdateGradeLevelDto } from '../models/gradelevel.model';

@Injectable({
  providedIn: 'root'
})
export class GradeLevelService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/GradeLevel`;

  getAllGradeLevels(): Observable<GradeLevelDto[]> {
    return this.http.get<GradeLevelDto[]>(this.apiUrl);
  }

  getGradeLevelById(gradeLevelId: string): Observable<GradeLevelDto> {
    return this.http.get<GradeLevelDto>(`${this.apiUrl}/${gradeLevelId}`);
  }

  createGradeLevel(createDto: CreateGradeLevelDto): Observable<GradeLevelDto> {
    return this.http.post<GradeLevelDto>(this.apiUrl, createDto);
  }

  updateGradeLevel(gradeLevelId: string, updateDto: UpdateGradeLevelDto): Observable<GradeLevelDto> {
    return this.http.put<GradeLevelDto>(`${this.apiUrl}/${gradeLevelId}`, updateDto);
  }

  deleteGradeLevel(gradeLevelId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${gradeLevelId}`);
  }
}
