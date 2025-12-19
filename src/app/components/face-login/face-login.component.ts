import { Component, OnInit, OnDestroy, ViewChild, ElementRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FaceRecognitionService } from '../../services/face-recognition.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-face-login',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './face-login.component.html',
  styleUrls: ['./face-login.component.css']
})
export class FaceLoginComponent implements OnInit, OnDestroy {
  @ViewChild('video', { static: false }) videoElement!: ElementRef<HTMLVideoElement>;
  @ViewChild('canvas', { static: false }) canvasElement!: ElementRef<HTMLCanvasElement>;

  private faceRecognitionService = inject(FaceRecognitionService);
  private authService = inject(AuthService);
  private router = inject(Router);

  stream: MediaStream | null = null;
  isAuthenticating = false;
  message = '';
  error = '';
  detectionInterval: any;
  faceDetected = false;

  async ngOnInit() {
    try {
      this.message = 'Loading face recognition models...';
      await this.faceRecognitionService.loadModels();
      this.message = 'Starting camera...';
      await this.startCamera();
      this.message = 'Position your face in the camera frame and click Login';
      this.startDetection();
    } catch (error: any) {
      this.error = error.message || 'Failed to initialize camera';
      console.error('Initialization error:', error);
    }
  }

  async startCamera() {
    try {
      this.stream = await this.faceRecognitionService.startCamera();

      setTimeout(() => {
        if (this.videoElement?.nativeElement) {
          this.videoElement.nativeElement.srcObject = this.stream;
        }
      }, 100);
    } catch (error) {
      throw new Error('Unable to access camera');
    }
  }

  startDetection() {
    this.detectionInterval = setInterval(async () => {
      if (this.videoElement?.nativeElement && this.videoElement.nativeElement.readyState === 4) {
        const detection = await this.faceRecognitionService.detectFace(this.videoElement.nativeElement);

        if (detection) {
          this.faceDetected = true;
          this.drawDetection(detection);
        } else {
          this.faceDetected = false;
          this.clearCanvas();
        }
      }
    }, 100);
  }

  drawDetection(detection: any) {
    if (!this.canvasElement?.nativeElement || !this.videoElement?.nativeElement) {
      return;
    }

    const canvas = this.canvasElement.nativeElement;
    const video = this.videoElement.nativeElement;

    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;

    const ctx = canvas.getContext('2d');
    if (ctx) {
      ctx.clearRect(0, 0, canvas.width, canvas.height);

      const box = detection.detection.box;
      ctx.strokeStyle = '#00ff00';
      ctx.lineWidth = 2;
      ctx.strokeRect(box.x, box.y, box.width, box.height);
    }
  }

  clearCanvas() {
    if (this.canvasElement?.nativeElement) {
      const canvas = this.canvasElement.nativeElement;
      const ctx = canvas.getContext('2d');
      if (ctx) {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
      }
    }
  }

  async loginWithFace() {
    if (!this.videoElement?.nativeElement) {
      this.error = 'Video element not ready';
      return;
    }

    this.isAuthenticating = true;
    this.message = 'Recognizing your face...';
    this.error = '';

    try {
      const descriptor = await this.faceRecognitionService.getFaceDescriptor(this.videoElement.nativeElement);

      if (!descriptor) {
        this.error = 'No face detected. Please ensure your face is clearly visible.';
        this.isAuthenticating = false;
        return;
      }

      this.message = 'Authenticating...';

      const faceLoginRequest = {
        faceDescriptor: Array.from(descriptor)
      };

      this.faceRecognitionService.loginWithFace(faceLoginRequest).subscribe({
        next: (response) => {
          localStorage.setItem('token', response.token);
          this.authService['setCurrentUser'](response.token);
          this.message = 'Login successful! Redirecting...';
          setTimeout(() => {
            this.navigateByRole();
          }, 1000);
        },
        error: (err) => {
          this.error = err.error?.message || 'Face not recognized. Please try again or use another login method.';
          this.isAuthenticating = false;
        }
      });
    } catch (error: any) {
      this.error = error.message || 'Failed to authenticate with face';
      this.isAuthenticating = false;
    }
  }

  cancel() {
    this.router.navigate(['/login']);
  }

  private navigateByRole() {
    if (this.authService.hasRole(['Admin'])) {
      this.router.navigate(['/admin']);
    } else if (this.authService.hasRole(['AcademicAffairs', 'Principal', 'DepartmentHead'])) {
      this.router.navigate(['/academic']);
    } else if (this.authService.hasRole(['SubjectTeacher'])) {
      this.router.navigate(['/teacher']);
    } else {
      this.error = 'You do not have access to the system';
      this.authService.logout();
      setTimeout(() => {
        this.router.navigate(['/login']);
      }, 2000);
    }
  }

  ngOnDestroy() {
    if (this.detectionInterval) {
      clearInterval(this.detectionInterval);
    }
    if (this.stream) {
      this.faceRecognitionService.stopCamera(this.stream);
    }
  }
}