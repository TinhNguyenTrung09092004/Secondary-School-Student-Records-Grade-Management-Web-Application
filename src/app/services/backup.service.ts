import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { BackupSchedule, BackupFile, CreateBackupRequest, BackupResponse } from '../models/backup.model';

@Injectable({
  providedIn: 'root'
})
export class BackupService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/backup`;

  getTables(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/tables`);
  }

  getBackupFiles(): Observable<BackupFile[]> {
    return this.http.get<BackupFile[]>(`${this.apiUrl}/files`);
  }

  getSchedule(): Observable<BackupSchedule> {
    return this.http.get<BackupSchedule>(`${this.apiUrl}/schedule`);
  }

  createBackup(request: CreateBackupRequest): Observable<BackupResponse> {
    return this.http.post<BackupResponse>(`${this.apiUrl}/create`, request);
  }

  updateSchedule(schedule: BackupSchedule): Observable<BackupResponse> {
    return this.http.post<BackupResponse>(`${this.apiUrl}/schedule`, schedule);
  }

  downloadBackup(fileName: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/download/${fileName}`, {
      responseType: 'blob'
    });
  }

  restoreBackup(fileName: string): Observable<BackupResponse> {
    return this.http.post<BackupResponse>(`${this.apiUrl}/restore/${fileName}`, {});
  }
}
