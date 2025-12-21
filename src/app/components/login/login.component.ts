import { Component, OnInit, inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
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
export class LoginComponent implements OnInit {
  email = '';
  password = '';
  error = '';
  private platformId = inject(PLATFORM_ID);
  private faceRecognitionService = inject(FaceRecognitionService);
  private msalInstance: any;
  faceLoginInProgress = false;

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit() {
    if (isPlatformBrowser(this.platformId)) {
      this.loadGoogleScript();
      this.loadMicrosoftScript();
    }
  }

  onLogin() {
    const credentials: LoginRequest = {
      email: this.email,
      password: this.password
    };

    this.authService.login(credentials).subscribe({
      next: () => this.navigateByRole(),
      error: (err) => this.error = 'Đăng nhập thất bại'
    });
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