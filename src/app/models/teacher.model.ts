export interface TeacherDto {
  teacherId: string;
  userId?: string;
  teacherName: string;
  address: string;
  phoneNumber: string;
  subjectId: string;
  departmentId?: string;
}

export interface CreateTeacherProfileDto {
  teacherId: string;
  teacherName: string;
  address: string;
  phoneNumber: string;
  subjectId: string;
  departmentId?: string;
}

export interface UpdateTeacherProfileDto {
  teacherName: string;
  address: string;
  phoneNumber: string;
  subjectId: string;
  departmentId?: string;
}
