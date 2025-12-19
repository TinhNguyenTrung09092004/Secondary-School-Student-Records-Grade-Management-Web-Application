export interface TeachingAssignmentDto {
  rowNumber: number;
  schoolYearId: string;
  schoolYearName: string;
  classId: string;
  className: string;
  subjectId: string;
  subjectName: string;
  teacherId: string;
  teacherName: string;
}

export interface CreateTeachingAssignmentDto {
  schoolYearId: string;
  classId: string;
  subjectId: string;
  teacherId: string;
}

export interface UpdateTeachingAssignmentDto {
  schoolYearId: string;
  classId: string;
  subjectId: string;
  teacherId: string;
}

export interface DepartmentTeacherDto {
  teacherId: string;
  teacherName: string;
  subjectId: string;
  subjectName: string;
  isHeadTeacher: boolean;
}

export interface DepartmentHeadCheckDto {
  isDepartmentHead: boolean;
}
