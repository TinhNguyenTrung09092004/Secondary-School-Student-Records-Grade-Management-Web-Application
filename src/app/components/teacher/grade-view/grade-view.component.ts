import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GradeService } from '../../../services/grade.service';
import {
  TeacherClassSubjectDto,
  StudentGradeViewDto,
  SemesterGradesSummary,
  GradeComponentDto
} from '../../../models/grade.model';

@Component({
  selector: 'app-grade-view',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './grade-view.component.html',
  styleUrl: './grade-view.component.css'
})
export class GradeViewComponent implements OnInit {
  private gradeService = inject(GradeService);

  teacherClasses: TeacherClassSubjectDto[] = [];
  studentGradesView: StudentGradeViewDto[] = [];

  selectedClass: TeacherClassSubjectDto | null = null;

  loading = false;
  errorMessage = '';

  // Track expanded rows (student ID + semester ID)
  expandedRows: Set<string> = new Set();

  ngOnInit(): void {
    this.loadTeacherClasses();
  }

  loadTeacherClasses(): void {
    this.loading = true;
    this.gradeService.getTeacherClasses().subscribe({
      next: (data) => {
        this.teacherClasses = data;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách lớp của bạn');
        this.loading = false;
      }
    });
  }

  onClassChange(event: Event): void {
    const select = event.target as HTMLSelectElement;
    const value = select.value;

    if (value) {
      const [classId, subjectId, schoolYearId] = value.split('|');
      this.selectedClass = this.teacherClasses.find(
        c => c.classId === classId && c.subjectId === subjectId && c.schoolYearId === schoolYearId
      ) || null;
      this.loadStudentGradesView();
    } else {
      this.selectedClass = null;
      this.studentGradesView = [];
    }
  }

  loadStudentGradesView(): void {
    if (!this.selectedClass) {
      return;
    }

    this.loading = true;
    this.expandedRows.clear();

    this.gradeService.getStudentGradesView(
      this.selectedClass.classId,
      this.selectedClass.subjectId,
      this.selectedClass.schoolYearId
    ).subscribe({
      next: (data) => {
        this.studentGradesView = data;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách điểm');
        this.loading = false;
      }
    });
  }

  toggleRow(studentId: string, semesterId: string): void {
    const key = `${studentId}-${semesterId}`;
    if (this.expandedRows.has(key)) {
      this.expandedRows.delete(key);
    } else {
      this.expandedRows.add(key);
    }
  }

  isRowExpanded(studentId: string, semesterId: string): boolean {
    return this.expandedRows.has(`${studentId}-${semesterId}`);
  }

  showError(message: string): void {
    this.errorMessage = message;
    setTimeout(() => this.errorMessage = '', 5000);
  }

  getClassSubjectKey(item: TeacherClassSubjectDto): string {
    return `${item.classId}|${item.subjectId}|${item.schoolYearId}`;
  }
}
