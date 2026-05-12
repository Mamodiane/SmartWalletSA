import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginRequest } from '../models/login-request.model';
import { RegisterRequest } from '../models/register-request.model';
import { AuthResponse } from '../models/auth-response.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7121/api/auth';

  constructor(private http: HttpClient) {}

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, request);
  }

  register(request: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, request);
  }

  saveToken(token: string): void {
    localStorage.setItem('smart_wallet_token', token);
  }

  getToken(): string | null {
    return localStorage.getItem('smart_wallet_token');
  }

  logout(): void {
    localStorage.removeItem('smart_wallet_token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}