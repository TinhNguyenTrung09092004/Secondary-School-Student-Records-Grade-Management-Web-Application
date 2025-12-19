import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { LoginRequest, LoginResponse, User } from '../models/auth.model';
import { environment } from '../../environments/environment';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/auth`;
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor() {
    const token = this.getToken();
    if (token) {
      this.setCurrentUser(token);
    }
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials)
      .pipe(
        tap(response => {
          localStorage.setItem('token', response.token);
          this.setCurrentUser(response.token);
        })
      );
  }

  logout(): void {
    localStorage.removeItem('token');
    this.currentUserSubject.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;

    try {
      const decoded: any = jwtDecode(token);
      return decoded.exp * 1000 > Date.now();
    } catch {
      return false;
    }
  }

  hasRole(roles: string[]): boolean {
    const user = this.currentUserSubject.value;
    if (!user) return false;
    return roles.some(role => user.roles.includes(role));
  }

  externalLogin(provider: string, idToken: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/external-login`, {
      provider,
      idToken
    }).pipe(
      tap(response => {
        localStorage.setItem('token', response.token);
        this.setCurrentUser(response.token);
      })
    );
  }

  forgotPassword(email: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/forgot-password`, { email });
  }

  resetPassword(email: string, token: string, newPassword: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/reset-password`, { email, token, newPassword });
  }

  completeAccountSetup(email: string, token: string, password: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/complete-account-setup`, { email, token, password });
  }

  private setCurrentUser(token: string): void {
    try {
      const decoded: any = jwtDecode(token);

      // Handle both short and full claim URIs
      const email = decoded.email ||
                    decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'];

      const userId = decoded.sub ||
                     decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];

      const roleClaim = decoded.role ||
                        decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

      const user: User = {
        id: userId,
        email: email,
        roles: roleClaim ? (Array.isArray(roleClaim) ? roleClaim : [roleClaim]) : []
      };
      this.currentUserSubject.next(user);
    } catch {
      this.currentUserSubject.next(null);
    }
  }
}