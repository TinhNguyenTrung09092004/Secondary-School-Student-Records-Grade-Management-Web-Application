import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './reset-password.component.html'
})
export class ResetPasswordComponent implements OnInit {
  email = '';
  token = '';
  newPassword = '';
  confirmPassword = '';
  message = '';
  error = '';
  isLoading = false;
  isAccountSetup = false; // Track whether this is account setup or password reset

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    // Get email and token from sessionStorage (not from URL)
    this.email = sessionStorage.getItem('pwd_email') || '';
    this.token = sessionStorage.getItem('pwd_token') || '';
    const type = sessionStorage.getItem('pwd_type') || 'reset';

    this.isAccountSetup = type === 'setup';

    // If no token in session, check URL params (fallback for old links)
    if (!this.email || !this.token) {
      this.route.queryParams.subscribe(params => {
        this.email = params['email'] || '';
        this.token = params['token'] || '';
      });
    }
  }

  onSubmit() {
    this.error = '';
    this.message = '';

    if (!this.newPassword || !this.confirmPassword) {
      this.error = 'Vui lòng nhập đầy đủ thông tin';
      return;
    }

    if (this.newPassword !== this.confirmPassword) {
      this.error = 'Mật khẩu xác nhận không khớp';
      return;
    }

    // Validate password requirements
    if (this.newPassword.length < 6) {
      this.error = 'Mật khẩu phải có ít nhất 6 ký tự';
      return;
    }

    if (!/[A-Z]/.test(this.newPassword)) {
      this.error = 'Mật khẩu phải có ít nhất 1 chữ hoa (A-Z)';
      return;
    }

    if (!/[a-z]/.test(this.newPassword)) {
      this.error = 'Mật khẩu phải có ít nhất 1 chữ thường (a-z)';
      return;
    }

    if (!/[^a-zA-Z0-9]/.test(this.newPassword)) {
      this.error = 'Mật khẩu phải có ít nhất 1 ký tự đặc biệt (!@#$%^&*)';
      return;
    }

    this.isLoading = true;

    // Choose the correct API endpoint based on whether this is account setup or password reset
    const apiCall = this.isAccountSetup
      ? this.authService.completeAccountSetup(this.email, this.token, this.newPassword)
      : this.authService.resetPassword(this.email, this.token, this.newPassword);

    apiCall.subscribe({
      next: (response) => {
        this.isLoading = false;
        this.message = response.message ||
          (this.isAccountSetup ? 'Tài khoản đã được thiết lập thành công' : 'Mật khẩu đã được đặt lại thành công');

        // Clear sessionStorage
        sessionStorage.removeItem('pwd_email');
        sessionStorage.removeItem('pwd_token');
        sessionStorage.removeItem('pwd_type');

        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (err) => {
        this.isLoading = false;
        this.error = err.error?.message ||
          (this.isAccountSetup ? 'Không thể thiết lập tài khoản. Liên kết có thể đã hết hạn' : 'Không thể đặt lại mật khẩu. Liên kết có thể đã hết hạn');
      }
    });
  }
}
