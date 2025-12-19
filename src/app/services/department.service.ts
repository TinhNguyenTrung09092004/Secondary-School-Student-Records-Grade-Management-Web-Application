import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { DepartmentDto, DepartmentDetailDto, CreateDepartmentDto, UpdateDepartmentDto } from '../models/department.model';

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Department`;

  getAllDepartments(): Observable<DepartmentDto[]> {
    return this.http.get<DepartmentDto[]>(this.apiUrl);
  }

  getDepartmentById(id: string): Observable<DepartmentDetailDto> {
    return this.http.get<DepartmentDetailDto>(`${this.apiUrl}/${id}`);
  }

  createDepartment(department: CreateDepartmentDto): Observable<DepartmentDto> {
    return this.http.post<DepartmentDto>(this.apiUrl, department);
  }

  updateDepartment(id: string, department: UpdateDepartmentDto): Observable<DepartmentDto> {
    return this.http.put<DepartmentDto>(`${this.apiUrl}/${id}`, department);
  }

  deleteDepartment(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
