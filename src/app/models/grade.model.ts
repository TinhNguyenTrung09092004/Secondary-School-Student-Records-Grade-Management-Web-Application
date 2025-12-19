export interface GradeDto {
  rowNumber: number;
  studentId: string;
  subjectId: string;
  semesterId: string;
  schoolYearId: string;
  classId: string;
  gradeTypeId: string;
  score: number | null;
  isComment: boolean;
  comment: string | null;
}

export interface CreateGradeDto {
  studentId: string;
  subjectId: string;
  semesterId: string;
  schoolYearId: string;
  classId: string;
  gradeTypeId: string;
  score: number | null;
  isComment: boolean;
  comment: string | null;
}

export interface UpdateGradeDto {
  score: number | null;
  isComment: boolean;
  comment: string | null;
}

export interface StudentGradeDto {
  studentId: string;
  studentName: string;
  grades: GradeDetailDto[];
}

export interface GradeDetailDto {
  rowNumber: number;
  gradeTypeId: string;
  gradeTypeName: string;
  coefficient: number;
  score: number | null;
  isComment: boolean;
  comment: string | null;
}

export interface TeacherClassSubjectDto {
  classId: string;
  className: string;
  subjectId: string;
  subjectName: string;
  schoolYearId: string;
  schoolYearName: string;
}

export interface SemesterDto {
  semesterId: string;
  semesterName: string;
  coefficient: number;
}

export interface GradeTypeDto {
  gradeTypeId: string;
  gradeTypeName: string;
  coefficient: number;
}

export interface CreateGradeTypeDto {
  gradeTypeId: string;
  gradeTypeName: string;
  coefficient: number;
}

export interface UpdateGradeTypeDto {
  gradeTypeId: string;
  gradeTypeName: string;
  coefficient: number;
}

// Interfaces for Grade Viewing Feature
export interface StudentGradeViewDto {
  studentId: string;
  studentName: string;
  semester1?: SemesterGradesSummary;
  semester2?: SemesterGradesSummary;
  yearAverage?: number;
  yearAverageDisplay?: string;
}

export interface SemesterGradesSummary {
  semesterId: string;
  average?: number;
  averageDisplay?: string;
  components: GradeComponentDto[];
}

export interface GradeComponentDto {
  gradeTypeId: string;
  gradeTypeName: string;
  coefficient: number;
  score?: number;
  isComment: boolean;
  comment?: string;
}
