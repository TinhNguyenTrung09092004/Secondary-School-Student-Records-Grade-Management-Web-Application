export interface SubjectDto {
  subjectId: string;
  subjectName: string;
  lessonCount: number;
  coefficient: number;
}

export interface CreateSubjectDto {
  subjectName: string;
  lessonCount: number;
  coefficient: number;
}

export interface UpdateSubjectDto {
  subjectName: string;
  lessonCount: number;
  coefficient: number;
}
