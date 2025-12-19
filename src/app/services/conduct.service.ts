import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ConductDto, CreateConductDto, UpdateConductDto } from '../models/conduct.model';

@Injectable({
  providedIn: 'root'
})
export class ConductService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/conduct`;

  getAllConduct(): Observable<ConductDto[]> {
    return this.http.get<ConductDto[]>(this.apiUrl);
  }

  getConductById(conductId: string): Observable<ConductDto> {
    return this.http.get<ConductDto>(`${this.apiUrl}/${conductId}`);
  }

  createConduct(createDto: CreateConductDto): Observable<ConductDto> {
    return this.http.post<ConductDto>(this.apiUrl, createDto);
  }

  updateConduct(conductId: string, updateDto: UpdateConductDto): Observable<ConductDto> {
    return this.http.put<ConductDto>(`${this.apiUrl}/${conductId}`, updateDto);
  }

  deleteConduct(conductId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${conductId}`);
  }
}