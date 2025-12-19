import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  UserDto,
  CreateUserDto,
  UpdateUserDto,
  UpdateUserPasswordDto,
  RoleDto,
  UserRoleUpdateDto
} from '../models/user-management.model';

@Injectable({
  providedIn: 'root'
})
export class UserManagementService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/usermanagement`;

  getAllUsers(): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(`${this.apiUrl}/users`);
  }

  getUserById(userId: string): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.apiUrl}/users/${userId}`);
  }

  createUser(createUserDto: CreateUserDto): Observable<UserDto> {
    return this.http.post<UserDto>(`${this.apiUrl}/users`, createUserDto);
  }

  updateUser(userId: string, updateUserDto: UpdateUserDto): Observable<UserDto> {
    return this.http.put<UserDto>(`${this.apiUrl}/users/${userId}`, updateUserDto);
  }

  updateUserPassword(userId: string, passwordDto: UpdateUserPasswordDto): Observable<any> {
    return this.http.put(`${this.apiUrl}/users/${userId}/password`, passwordDto);
  }

  deleteUser(userId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/users/${userId}`);
  }

  cancelDeletion(userId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/users/${userId}/cancel-deletion`, {});
  }

  toggleUserLockout(userId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/users/${userId}/toggle-lockout`, {});
  }

  getAllRoles(): Observable<RoleDto[]> {
    return this.http.get<RoleDto[]>(`${this.apiUrl}/roles`);
  }

  updateUserRoles(userId: string, roleUpdateDto: UserRoleUpdateDto): Observable<any> {
    return this.http.put(`${this.apiUrl}/users/${userId}/roles`, roleUpdateDto);
  }
}
