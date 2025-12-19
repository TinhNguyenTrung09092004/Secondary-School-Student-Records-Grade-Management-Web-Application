export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  email: string;
  roles: string[];
}

export interface User {
  id?: string;
  email: string;
  roles: string[];
}

export interface EnrollFaceRequest {
  email: string;
  faceDescriptor: number[];
}

export interface FaceLoginRequest {
  faceDescriptor: number[];
}

export interface FaceEnrollmentStatus {
  isEnrolled: boolean;
  enrolledAt?: Date;
}
