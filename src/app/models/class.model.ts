export interface ClassDto {
  classId: string;
  className: string;
  gradeLevelId: string;
  gradeLevelName: string;
  schoolYearId: string;
  schoolYearName: string;
  classSize: number;
  currentStudentCount: number;
  teacherId?: string;
  teacherName?: string;
}

export interface CreateClassDto {
  classId: string;
  className: string;
  gradeLevelId: string;
  schoolYearId: string;
  classSize: number;
}

export interface UpdateClassDto {
  classId: string;
  className: string;
  gradeLevelId: string;
  schoolYearId: string;
  classSize: number;
}
