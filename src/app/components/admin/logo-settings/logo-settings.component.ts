import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-logo-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './logo-settings.component.html'
})
export class LogoSettingsComponent {
  logoUrl = '';
  currentLogoUrl = '';
  uploading = false;
  message = '';
  messageType: 'success' | 'error' = 'success';

  constructor(private http: HttpClient) {
    this.loadCurrentLogo();
  }

  loadCurrentLogo() {
    this.http.get<any>(`${environment.apiUrl}/api/SystemSettings/logo`).subscribe({
      next: (response) => {
        this.currentLogoUrl = response.logoUrl;
        this.logoUrl = response.logoUrl;
      },
      error: (err) => {
        console.error('Error loading logo:', err);
        this.message = 'Không thể tải logo hiện tại. Vui lòng kiểm tra API.';
        this.messageType = 'error';
        // Set default logo
        this.currentLogoUrl = 'https://res.cloudinary.com/dxfubqntx/image/upload/v1766312494/LOGO_k6u5iq.png';
        this.logoUrl = this.currentLogoUrl;
      }
    });
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (!file) return;

    if (!file.type.startsWith('image/')) {
      this.message = 'Vui lòng chọn file hình ảnh';
      this.messageType = 'error';
      return;
    }

    this.uploadToCloudinary(file);
  }

  uploadToCloudinary(file: File) {
    this.uploading = true;
    this.message = 'Đang tải lên...';
    this.messageType = 'success';

    const formData = new FormData();
    formData.append('file', file);
    formData.append('upload_preset', 'ml_default');

    // Use XMLHttpRequest to bypass Angular HTTP interceptor
    const xhr = new XMLHttpRequest();

    xhr.onload = () => {
      if (xhr.status === 200) {
        const response = JSON.parse(xhr.responseText);
        this.logoUrl = response.secure_url;
        this.message = 'Tải lên thành công! Nhấn "Lưu Logo" để áp dụng.';
        this.messageType = 'success';
        this.uploading = false;
      } else {
        const error = JSON.parse(xhr.responseText);
        this.message = `Tải lên thất bại: ${error.error?.message || 'Không rõ lỗi'}`;
        this.messageType = 'error';
        this.uploading = false;
      }
    };

    xhr.onerror = () => {
      this.message = 'Tải lên thất bại. Vui lòng kiểm tra kết nối mạng.';
      this.messageType = 'error';
      this.uploading = false;
    };

    xhr.open('POST', 'https://api.cloudinary.com/v1_1/dxfubqntx/image/upload');
    xhr.send(formData);
  }

  saveLogo() {
    if (!this.logoUrl) {
      this.message = 'Vui lòng nhập URL logo';
      this.messageType = 'error';
      return;
    }

    this.http.post<any>(`${environment.apiUrl}/api/SystemSettings/logo`, { logoUrl: this.logoUrl }).subscribe({
      next: () => {
        this.message = 'Cập nhật logo thành công!';
        this.messageType = 'success';
        this.currentLogoUrl = this.logoUrl;
      },
      error: () => {
        this.message = 'Cập nhật logo thất bại';
        this.messageType = 'error';
      }
    });
  }
}
