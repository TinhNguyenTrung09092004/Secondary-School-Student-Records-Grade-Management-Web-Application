import { Component, OnInit, OnDestroy, ViewChild, ElementRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { FaceRecognitionService } from '../../services/face-recognition.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-face-enrollment',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './face-enrollment.component.html',
  styleUrls: ['./face-enrollment.component.css']
})
export class FaceEnrollmentComponent implements OnInit, OnDestroy {
  @ViewChild('video', { static: false }) videoElement!: ElementRef<HTMLVideoElement>;
  @ViewChild('canvas', { static: false }) canvasElement!: ElementRef<HTMLCanvasElement>;

  private faceRecognitionService = inject(FaceRecognitionService);
  private authService = inject(AuthService);
  private router = inject(Router);

  stream: MediaStream | null = null;
  isDetecting = false;
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
      this.message = 'Position your face in the camera frame';
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

  async enrollFace() {
    if (!this.videoElement?.nativeElement) {
      this.error = 'Video element not ready';
      return;
    }

    this.isDetecting = true;
    this.message = 'Capturing your face...';
    this.error = '';

    try {
      const descriptor = await this.faceRecognitionService.getFaceDescriptor(this.videoElement.nativeElement);

      if (!descriptor) {
        this.error = 'No face detected. Please ensure your face is clearly visible.';
        this.isDetecting = false;
        return;
      }

      const user = await firstValueFrom(this.authService.currentUser$);
      if (!user || !user.email) {
        this.error = 'User not authenticated';
        this.isDetecting = false;
        return;
      }

      this.message = 'Enrolling face...';

      const enrollRequest = {
        email: user.email,
        faceDescriptor: Array.from(descriptor)
      };

      this.faceRecognitionService.enrollFace(enrollRequest).subscribe({
        next: (response) => {
          this.message = 'Face enrolled successfully! Redirecting...';
          setTimeout(() => {
            this.navigateBack();
          }, 2000);
        },
        error: (err) => {
          this.error = err.error?.message || 'Failed to enroll face';
          this.isDetecting = false;
        }
      });
    } catch (error: any) {
      this.error = error.message || 'Failed to capture face';
      this.isDetecting = false;
    }
  }

  navigateBack() {
    this.authService.currentUser$.subscribe(user => {
      if (user) {
        if (user.roles.includes('Admin')) {
          this.router.navigate(['/admin']);
        } else if (user.roles.some((r: string) => ['AcademicAffairs', 'Principal', 'DepartmentHead'].includes(r))) {
          this.router.navigate(['/academic']);
        } else if (user.roles.includes('SubjectTeacher')) {
          this.router.navigate(['/teacher']);
        }
      }
    });
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