export interface SemesterDto {
  semesterId: string;
  semesterName: string;
  coefficient: number;
}

export interface CreateSemesterDto {
  semesterId: string;
  semesterName: string;
  coefficient: number;
}

export interface UpdateSemesterDto {
  semesterId: string;
  semesterName: string;
  coefficient: number;
}
