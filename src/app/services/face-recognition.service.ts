import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import * as faceapi from 'face-api.js';
import { environment } from '../../environments/environment';
import { EnrollFaceRequest, FaceLoginRequest, FaceEnrollmentStatus, LoginResponse } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class FaceRecognitionService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/facerecognition`;
  private modelsLoaded = false;

  async loadModels(): Promise<void> {
    if (this.modelsLoaded) {
      return;
    }

    const MODEL_URL = '/models';

    try {
      console.log('Loading face-api.js models from:', MODEL_URL);
      await Promise.all([
        faceapi.nets.tinyFaceDetector.loadFromUri(MODEL_URL),
        faceapi.nets.faceLandmark68Net.loadFromUri(MODEL_URL),
        faceapi.nets.faceRecognitionNet.loadFromUri(MODEL_URL)
      ]);
      this.modelsLoaded = true;
      console.log('Face-api.js models loaded successfully');
    } catch (error) {
      console.error('Error loading face-api.js models:', error);
      console.error('Make sure model files are in the public/models directory');
      throw new Error('Failed to load face recognition models. Please ensure model files are downloaded.');
    }
  }

  async detectFace(videoElement: HTMLVideoElement): Promise<faceapi.WithFaceDescriptor<faceapi.WithFaceLandmarks<faceapi.WithFaceDetection<{}>>> | null> {
    if (!this.modelsLoaded) {
      await this.loadModels();
    }

    const detection = await faceapi
      .detectSingleFace(videoElement, new faceapi.TinyFaceDetectorOptions())
      .withFaceLandmarks()
      .withFaceDescriptor();

    return detection || null;
  }

  async getFaceDescriptor(videoElement: HTMLVideoElement): Promise<Float32Array | null> {
    const detection = await this.detectFace(videoElement);
    return detection ? detection.descriptor : null;
  }

  enrollFace(enrollRequest: EnrollFaceRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/enroll`, enrollRequest);
  }

  loginWithFace(faceLoginRequest: FaceLoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, faceLoginRequest);
  }

  getEnrollmentStatus(email: string): Observable<FaceEnrollmentStatus> {
    return this.http.get<FaceEnrollmentStatus>(`${this.apiUrl}/status/${email}`);
  }

  async startCamera(): Promise<MediaStream> {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({
        video: {
          width: 640,
          height: 480,
          facingMode: 'user'
        }
      });
      return stream;
    } catch (error) {
      console.error('Error accessing camera:', error);
      throw new Error('Unable to access camera. Please ensure camera permissions are granted.');
    }
  }

  stopCamera(stream: MediaStream): void {
    stream.getTracks().forEach(track => track.stop());
  }
}