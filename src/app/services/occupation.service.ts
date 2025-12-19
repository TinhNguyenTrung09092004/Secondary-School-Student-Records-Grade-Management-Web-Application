import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { OccupationDto, CreateOccupationDto, UpdateOccupationDto } from '../models/occupation.model';

@Injectable({
  providedIn: 'root'
})
export class OccupationService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/occupation`;

  getAllOccupation(): Observable<OccupationDto[]> {
    return this.http.get<OccupationDto[]>(this.apiUrl);
  }

  getOccupationById(occupationId: string): Observable<OccupationDto> {
    return this.http.get<OccupationDto>(`${this.apiUrl}/${occupationId}`);
  }

  createOccupation(createDto: CreateOccupationDto): Observable<OccupationDto> {
    return this.http.post<OccupationDto>(this.apiUrl, createDto);
  }

  updateOccupation(occupationId: string, updateDto: UpdateOccupationDto): Observable<OccupationDto> {
    return this.http.put<OccupationDto>(`${this.apiUrl}/${occupationId}`, updateDto);
  }

  deleteOccupation(occupationId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${occupationId}`);
  }
}
