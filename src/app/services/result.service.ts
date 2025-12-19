import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ResultDto, CreateResultDto, UpdateResultDto } from '../models/result.model';

@Injectable({
  providedIn: 'root'
})
export class ResultService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Result`;

  getAll(): Observable<ResultDto[]> {
    return this.http.get<ResultDto[]>(this.apiUrl);
  }

  getById(id: string): Observable<ResultDto> {
    return this.http.get<ResultDto>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateResultDto): Observable<ResultDto> {
    return this.http.post<ResultDto>(this.apiUrl, dto);
  }

  update(id: string, dto: UpdateResultDto): Observable<ResultDto> {
    return this.http.put<ResultDto>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
