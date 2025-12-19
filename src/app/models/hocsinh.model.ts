export interface StudentDto {
  studentId: string;
  fullName: string;
  gender: boolean;
  dateOfBirth: string;
  address: string;
  email: string;
  ethnicityId: string;
  religionId: string;
  fatherName: string;
  fatherOccupationId: string;
  motherName: string;
  motherOccupationId: string;
}

export interface CreateStudentDto {
  studentId: string;
  fullName: string;
  gender: boolean;
  dateOfBirth: string;
  address: string;
  email: string;
  ethnicityId: string;
  religionId: string;
  fatherName: string;
  fatherOccupationId: string;
  motherName: string;
  motherOccupationId: string;
}

export interface UpdateStudentDto {
  fullName: string;
  gender: boolean;
  dateOfBirth: string;
  address: string;
  email: string;
  ethnicityId: string;
  religionId: string;
  fatherName: string;
  fatherOccupationId: string;
  motherName: string;
  motherOccupationId: string;
}
