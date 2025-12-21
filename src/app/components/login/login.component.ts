import { Component, OnInit, inject, PLATFORM_ID, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';
import { FaceRecognitionService } from '../../services/face-recognition.service';
import { Router, RouterLink } from '@angular/router';
import { LoginRequest } from '../../models/auth.model';
import { environment } from '../../../environments/environment';

declare var google: any;
declare var msal: any;

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit, AfterViewInit {
  @ViewChild('captchaCanvas') captchaCanvas?: ElementRef<HTMLCanvasElement>;

  email = '';
  password = '';
  error = '';
  logoUrl = 'https://res.cloudinary.com/dxfubqntx/image/upload/v1766312494/LOGO_k6u5iq.png';
  showCaptcha = false;
  captchaQuestion = '';
  captchaToken = '';
  captchaAnswer = '';
  private platformId = inject(PLATFORM_ID);
  private faceRecognitionService = inject(FaceRecognitionService);
  private msalInstance: any;
  faceLoginInProgress = false;

  constructor(private authService: AuthService, private router: Router, private http: HttpClient) {}

  ngAfterViewInit() {
    if (this.showCaptcha && this.captchaCanvas) {
      this.drawCaptcha();
    }
  }

  ngOnInit() {
    this.loadLogo();
    if (isPlatformBrowser(this.platformId)) {
      this.loadGoogleScript();
      this.loadMicrosoftScript();
    }
  }

  private loadLogo() {
    this.http.get<any>(`${environment.apiUrl}/api/SystemSettings/logo`).subscribe({
      next: (response) => {
        if (response.logoUrl) {
          this.logoUrl = response.logoUrl;
        }
      },
      error: () => {
        // Use default logo if API fails
        console.log('Failed to load logo from API, using default');
      }
    });
  }

  onLogin() {
    const credentials: any = {
      email: this.email,
      password: this.password
    };

    if (this.showCaptcha) {
      credentials.captchaToken = this.captchaToken;
      credentials.captchaAnswer = this.captchaAnswer;
    }

    this.authService.login(credentials).subscribe({
      next: () => this.navigateByRole(),
      error: (err) => {
        this.error = err.error?.message || 'Đăng nhập thất bại';

        if (err.error?.requiresCaptcha) {
          this.loadCaptcha();
        }
      }
    });
  }

  private loadCaptcha() {
    this.http.get<any>(`${environment.apiUrl}/api/auth/captcha`).subscribe({
      next: (response) => {
        this.showCaptcha = true;
        this.captchaQuestion = response.question;
        this.captchaToken = response.token;
        this.captchaAnswer = '';
        setTimeout(() => this.drawCaptcha(), 100);
      },
      error: () => {
        this.error = 'Không thể tải captcha';
      }
    });
  }

  refreshCaptcha() {
    this.loadCaptcha();
  }

  private drawCaptcha() {
    if (!this.captchaCanvas) return;

    const canvas = this.captchaCanvas.nativeElement;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    // Clear canvas
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Background with gradient
    const gradient = ctx.createLinearGradient(0, 0, canvas.width, canvas.height);
    gradient.addColorStop(0, '#f0f0f0');
    gradient.addColorStop(1, '#e0e0e0');
    ctx.fillStyle = gradient;
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    // Add noise lines
    for (let i = 0; i < 5; i++) {
      ctx.strokeStyle = `rgba(${Math.random() * 255}, ${Math.random() * 255}, ${Math.random() * 255}, 0.3)`;
      ctx.lineWidth = 1;
      ctx.beginPath();
      ctx.moveTo(Math.random() * canvas.width, Math.random() * canvas.height);
      ctx.lineTo(Math.random() * canvas.width, Math.random() * canvas.height);
      ctx.stroke();
    }

    // Draw distorted text
    const text = this.captchaQuestion;
    const charSpacing = canvas.width / (text.length + 1);

    for (let i = 0; i < text.length; i++) {
      const char = text[i];
      const x = charSpacing * (i + 1);
      const y = 35 + (Math.random() - 0.5) * 10;
      const angle = (Math.random() - 0.5) * 0.3;

      ctx.save();
      ctx.translate(x, y);
      ctx.rotate(angle);
      ctx.font = `bold ${24 + Math.random() * 6}px Arial`;
      ctx.fillStyle = `rgb(${Math.random() * 100}, ${Math.random() * 100}, ${Math.random() * 100})`;
      ctx.fillText(char, 0, 0);
      ctx.restore();
    }

    // Add noise dots
    for (let i = 0; i < 50; i++) {
      ctx.fillStyle = `rgba(${Math.random() * 255}, ${Math.random() * 255}, ${Math.random() * 255}, 0.3)`;
      ctx.fillRect(Math.random() * canvas.width, Math.random() * canvas.height, 2, 2);
    }
  }

  private loadGoogleScript() {
    const script = document.createElement('script');
    script.src = 'https://accounts.google.com/gsi/client';
    script.async = true;
    script.defer = true;
    script.onload = () => this.initializeGoogle();
    document.body.appendChild(script);
  }

  private loadMicrosoftScript() {
    const script = document.createElement('script');
    script.src = 'https://alcdn.msauth.net/browser/2.30.0/js/msal-browser.min.js';
    script.async = true;
    script.defer = true;
    script.onload = () => this.initializeMicrosoft();
    document.body.appendChild(script);
  }

  private async initializeMicrosoft() {
    const msalConfig = {
      auth: {
        clientId: environment.microsoftClientId,
        authority: 'https://login.microsoftonline.com/common',
        redirectUri: window.location.origin
      },
      cache: {
        cacheLocation: 'sessionStorage',
        storeAuthStateInCookie: false
      }
    };

    this.msalInstance = new msal.PublicClientApplication(msalConfig);
    await this.msalInstance.initialize();
  }

  private initializeGoogle() {
    google.accounts.id.initialize({
      client_id: environment.googleClientId,
      callback: (response: any) => this.handleGoogleLogin(response)
    });

    // Render button in hidden container
    const container = document.getElementById('google-button');
    if (container) {
      google.accounts.id.renderButton(container, {
        theme: 'outline',
        size: 'large'
      });
    }

    // Cancel auto-prompt to prevent One Tap badge
    google.accounts.id.cancel();
  }

  loginWithGoogle() {
    // Trigger the hidden Google button click
    const container = document.getElementById('google-button');
    if (container) {
      const googleBtn = container.querySelector('div[role="button"]') as HTMLElement;
      if (googleBtn) {
        googleBtn.click();
      }
    }
  }

  private handleGoogleLogin(response: any) {
    this.authService.externalLogin('google', response.credential).subscribe({
      next: () => this.navigateByRole(),
      error: () => this.error = 'Đăng nhập Google thất bại'
    });
  }

  async loginWithMicrosoft() {
    if (!this.msalInstance) {
      this.error = 'Microsoft đang tải, vui lòng thử lại';
      return;
    }

    try {
      const response = await this.msalInstance.loginPopup({
        scopes: ['user.read'],
        prompt: 'select_account'
      });

      console.log('Microsoft login response:', response);

      // Use the idToken instead of username
      const idToken = response.idToken;

      if (!idToken) {
        console.error('No idToken in response');
        this.error = 'Không nhận được token từ Microsoft';
        return;
      }

      this.authService.externalLogin('microsoft', idToken).subscribe({
        next: () => {
          console.log('Backend authentication successful');
          this.navigateByRole();
        },
        error: (err: any) => {
          console.error('Backend authentication error:', err);
          this.error = err.error?.message || 'Đăng nhập Microsoft thất bại';
        }
      });
    } catch (error: any) {
      console.error('Microsoft popup error:', error);
      if (error.errorCode === 'user_cancelled') {
        this.error = 'Đăng nhập bị hủy';
      } else {
        this.error = error.errorMessage || 'Đăng nhập Microsoft thất bại';
      }
    }
  }

  async loginWithFace() {
    this.router.navigate(['/face-login']);
  }

  private navigateByRole() {
    if (this.authService.hasRole(['Admin'])) {
      this.router.navigate(['/admin']);
    } else if (this.authService.hasRole(['AcademicAffairs', 'Principal', 'DepartmentHead'])) {
      this.router.navigate(['/academic']);
    } else if (this.authService.hasRole(['SubjectTeacher'])) {
    this.router.navigate(['/teacher'])
    } else {
      this.error = 'Bạn chưa có quyền truy cập vào hệ thống';
      this.authService.logout();
    }
  }
}