export interface DepartmentDto {
  departmentId: string;
  departmentName: string;
  headTeacherId?: string;
  headTeacherName?: string;
  teacherCount: number;
}

export interface DepartmentDetailDto {
  departmentId: string;
  departmentName: string;
  headTeacherId?: string;
  headTeacherName?: string;
  teachers: TeacherInDepartmentDto[];
}

export interface TeacherInDepartmentDto {
  teacherId: string;
  teacherName: string;
  subjectName: string;
}

export interface CreateDepartmentDto {
  departmentId: string;
  departmentName: string;
  headTeacherId?: string;
}

export interface UpdateDepartmentDto {
  departmentName: string;
  headTeacherId?: string;
}
