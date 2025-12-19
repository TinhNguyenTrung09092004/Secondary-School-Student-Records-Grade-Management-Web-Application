export interface ClassAssignmentDto {
  schoolYearId: string;
  schoolYearName: string;
  gradeLevelId: string;
  gradeLevelName: string;
  classId: string;
  className: string;
  studentId: string;
  studentName: string;
}

export interface StudentInClassDto {
  studentId: string;
  fullName: string;
  email: string;
  gender: boolean;
}

export interface ClassInfoDto {
  classId: string;
  className: string;
  gradeLevelId: string;
  gradeLevelName: string;
  schoolYearId: string;
  schoolYearName: string;
  classSize: number;
  currentStudentCount: number;
  teacherId: string;
  teacherName: string;
}

export interface AssignStudentToClassDto {
  schoolYearId: string;
  gradeLevelId: string;
  classId: string;
  studentId: string;
}

export interface BulkAssignStudentsDto {
  schoolYearId: string;
  gradeLevelId: string;
  classId: string;
  studentIds: string[];
}

export interface RemoveStudentFromClassDto {
  schoolYearId: string;
  gradeLevelId: string;
  classId: string;
  studentId: string;
}
