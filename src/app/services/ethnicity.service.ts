import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { EthnicityDto, CreateEthnicityDto, UpdateEthnicityDto } from '../models/ethnicity.model';

@Injectable({
  providedIn: 'root'
})
export class EthnicityService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/ethnicity`;

  getAllEthnicity(): Observable<EthnicityDto[]> {
    return this.http.get<EthnicityDto[]>(this.apiUrl);
  }

  getEthnicityById(ethnicityId: string): Observable<EthnicityDto> {
    return this.http.get<EthnicityDto>(`${this.apiUrl}/${ethnicityId}`);
  }

  createEthnicity(createDto: CreateEthnicityDto): Observable<EthnicityDto> {
    return this.http.post<EthnicityDto>(this.apiUrl, createDto);
  }

  updateEthnicity(ethnicityId: string, updateDto: UpdateEthnicityDto): Observable<EthnicityDto> {
    return this.http.put<EthnicityDto>(`${this.apiUrl}/${ethnicityId}`, updateDto);
  }

  deleteEthnicity(ethnicityId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${ethnicityId}`);
  }
}
