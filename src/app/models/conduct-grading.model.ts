export interface StudentConductDto {
  studentId: string;
  studentName: string;
  classId: string;
  className: string;
  schoolYearId: string;
  semester1ConductId?: string;
  semester1ConductName?: string;
  semester2ConductId?: string;
  semester2ConductName?: string;
  yearEndConductId?: string;
  yearEndConductName?: string;
}

export interface UpdateStudentConductDto {
  studentId: string;
  classId: string;
  schoolYearId: string;
  semesterId: string;
  conductId: string;
}

export interface ClassForConductGradingDto {
  classId: string;
  className: string;
  schoolYearId: string;
  schoolYearName: string;
  studentCount: number;
}