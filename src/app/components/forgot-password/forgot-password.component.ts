import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './forgot-password.component.html'
})
export class ForgotPasswordComponent {
  email = '';
  message = '';
  error = '';
  isLoading = false;

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit() {
    if (!this.email) {
      this.error = 'Vui lòng nhập email';
      return;
    }

    this.isLoading = true;
    this.error = '';
    this.message = '';

    this.authService.forgotPassword(this.email).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.message = response.message || 'Nếu email tồn tại, liên kết đặt lại mật khẩu đã được gửi';
        this.email = '';
      },
      error: (err) => {
        this.isLoading = false;
        this.error = 'Có lỗi xảy ra, vui lòng thử lại';
      }
    });
  }
}
