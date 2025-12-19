import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ReligionDto, CreateReligionDto, UpdateReligionDto } from '../models/religion.model';

@Injectable({
  providedIn: 'root'
})
export class ReligionService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/religion`;

  getAllReligion(): Observable<ReligionDto[]> {
    return this.http.get<ReligionDto[]>(this.apiUrl);
  }

  getReligionById(religionId: string): Observable<ReligionDto> {
    return this.http.get<ReligionDto>(`${this.apiUrl}/${religionId}`);
  }

  createReligion(createDto: CreateReligionDto): Observable<ReligionDto> {
    return this.http.post<ReligionDto>(this.apiUrl, createDto);
  }

  updateReligion(religionId: string, updateDto: UpdateReligionDto): Observable<ReligionDto> {
    return this.http.put<ReligionDto>(`${this.apiUrl}/${religionId}`, updateDto);
  }

  deleteReligion(religionId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${religionId}`);
  }
}
