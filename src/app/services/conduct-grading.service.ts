import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { StudentConductDto, UpdateStudentConductDto, ClassForConductGradingDto } from '../models/conduct-grading.model';

@Injectable({
  providedIn: 'root'
})
export class ConductGradingService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/conductgrading`;

  getMyClasses(): Observable<ClassForConductGradingDto[]> {
    return this.http.get<ClassForConductGradingDto[]>(`${this.apiUrl}/my-classes`);
  }

  getStudentsForGrading(classId: string, schoolYearId: string): Observable<StudentConductDto[]> {
    return this.http.get<StudentConductDto[]>(`${this.apiUrl}/students`, {
      params: { classId, schoolYearId }
    });
  }

  updateStudentConduct(updateDto: UpdateStudentConductDto): Observable<any> {
    return this.http.put(`${this.apiUrl}/update-conduct`, updateDto);
  }
}
