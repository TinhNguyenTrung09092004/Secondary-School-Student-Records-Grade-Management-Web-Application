import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GradeService } from '../../../services/grade.service';
import {
  TeacherClassSubjectDto,
  StudentGradeDto,
  SemesterDto,
  GradeTypeDto,
  CreateGradeDto,
  UpdateGradeDto,
  GradeDetailDto
} from '../../../models/grade.model';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-grade-entry',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './grade-entry.component.html',
  styleUrls: ['./grade-entry.component.css']
})
export class GradeEntryComponent implements OnInit {
  private gradeService = inject(GradeService);

  teacherClasses: TeacherClassSubjectDto[] = [];
  semesters: SemesterDto[] = [];
  gradeTypes: GradeTypeDto[] = [];
  studentGrades: StudentGradeDto[] = [];

  selectedClass: TeacherClassSubjectDto | null = null;
  selectedSemester: SemesterDto | null = null;

  loading = false;
  errorMessage = '';
  successMessage = '';

  editingCell: { studentId: string; gradeTypeId: string } | null = null;
  tempScore: string = '';
  isCommentMode: boolean = false;
  showImportDialog: boolean = false;
  importFile: File | null = null;
  importScoreType: 'score' | 'comment' = 'score';
  showExportDialog: boolean = false;
  exportFormat: string = '';

  // Pagination
  currentPage: number = 1;
  pageSize: number = 10;
  totalPages: number = 1;

  // Search
  searchQuery: string = '';

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading = true;
    forkJoin({
      classes: this.gradeService.getTeacherClasses(),
      semesters: this.gradeService.getSemesters(),
      gradeTypes: this.gradeService.getGradeTypes()
    }).subscribe({
      next: (data) => {
        this.teacherClasses = data.classes;
        this.semesters = data.semesters;
        // Sort grade types by coefficient (1, 2, 3)
        this.gradeTypes = data.gradeTypes.sort((a, b) => a.coefficient - b.coefficient);
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải dữ liệu');
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
      this.loadStudentGrades();
    } else {
      this.selectedClass = null;
      this.studentGrades = [];
    }
  }

  onSemesterChange(event: Event): void {
    const select = event.target as HTMLSelectElement;
    const value = select.value;

    if (value) {
      this.selectedSemester = this.semesters.find(s => s.semesterId === value) || null;
      this.loadStudentGrades();
    } else {
      this.selectedSemester = null;
      this.studentGrades = [];
    }
  }

  loadStudentGrades(): void {
    if (!this.selectedClass || !this.selectedSemester) {
      return;
    }

    this.loading = true;
    this.currentPage = 1; // Reset to first page when loading new data
    this.gradeService.getStudentGrades(
      this.selectedClass.classId,
      this.selectedClass.subjectId,
      this.selectedSemester.semesterId,
      this.selectedClass.schoolYearId
    ).subscribe({
      next: (grades) => {
        this.studentGrades = grades;
        this.updateTotalPages();
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách học sinh');
        this.loading = false;
      }
    });
  }

  toggleCommentMode(): void {
    this.isCommentMode = !this.isCommentMode;
  }

  startEdit(studentId: string, gradeTypeId: string, currentScore: number | null, currentComment: string | null = null): void {
    this.editingCell = { studentId, gradeTypeId };
    if (this.isCommentMode) {
      this.tempScore = currentComment || '';
    } else {
      this.tempScore = currentScore !== null ? currentScore.toString() : '';
    }
  }

  cancelEdit(): void {
    this.editingCell = null;
    this.tempScore = '';
  }

  saveGrade(student: StudentGradeDto, gradeDetail: GradeDetailDto | undefined): void {
    if (!this.selectedClass || !this.selectedSemester || !this.editingCell) {
      return;
    }

    let score: number | null = null;
    let comment: string | null = null;

    if (this.isCommentMode) {
      // Validate comment
      if (!this.tempScore || (this.tempScore !== 'Pass' && this.tempScore !== 'Fail')) {
        this.showError('Nhận xét phải là "Pass" hoặc "Fail"');
        return;
      }
      comment = this.tempScore;
    } else {
      // Validate score
      score = parseFloat(this.tempScore);
      if (isNaN(score) || score < 0 || score > 10) {
        this.showError('Điểm phải là số từ 0 đến 10');
        return;
      }
    }

    if (gradeDetail && gradeDetail.rowNumber > 0) {
      // Update existing grade
      const updateDto: UpdateGradeDto = {
        score: score,
        isComment: this.isCommentMode,
        comment: comment
      };

      this.loading = true;
      this.gradeService.updateGrade(gradeDetail.rowNumber, updateDto).subscribe({
        next: () => {
          gradeDetail.score = score;
          gradeDetail.isComment = this.isCommentMode;
          gradeDetail.comment = comment;
          this.showSuccess('Cập nhật điểm thành công');
          this.cancelEdit();
          this.loading = false;
        },
        error: (error) => {
          this.showError(error.error?.message || 'Không thể cập nhật điểm');
          this.loading = false;
        }
      });
    } else {
      // Create new grade
      const createDto: CreateGradeDto = {
        studentId: student.studentId,
        subjectId: this.selectedClass.subjectId,
        semesterId: this.selectedSemester.semesterId,
        schoolYearId: this.selectedClass.schoolYearId,
        classId: this.selectedClass.classId,
        gradeTypeId: this.editingCell.gradeTypeId,
        score: score,
        isComment: this.isCommentMode,
        comment: comment
      };

      this.loading = true;
      this.gradeService.createGrade(createDto).subscribe({
        next: (grade) => {
          // Find or create the grade detail in the student's grades array
          let newGradeDetail = student.grades.find(g => g.gradeTypeId === this.editingCell!.gradeTypeId);
          if (!newGradeDetail) {
            // Add new grade detail to student's grades array
            const gradeType = this.gradeTypes.find(gt => gt.gradeTypeId === this.editingCell!.gradeTypeId);
            newGradeDetail = {
              rowNumber: grade.rowNumber,
              gradeTypeId: this.editingCell!.gradeTypeId,
              gradeTypeName: gradeType?.gradeTypeName || '',
              coefficient: gradeType?.coefficient || 1,
              score: score,
              isComment: this.isCommentMode,
              comment: comment
            };
            student.grades.push(newGradeDetail);
          } else {
            newGradeDetail.rowNumber = grade.rowNumber;
            newGradeDetail.score = score;
            newGradeDetail.isComment = this.isCommentMode;
            newGradeDetail.comment = comment;
          }
          this.showSuccess('Thêm điểm thành công');
          this.cancelEdit();
          this.loading = false;
        },
        error: (error) => {
          this.showError(error.error?.message || 'Không thể thêm điểm');
          this.loading = false;
        }
      });
    }
  }

  deleteGrade(gradeDetail: GradeDetailDto): void {
    if (gradeDetail.rowNumber === 0) {
      return;
    }

    if (!confirm('Bạn có chắc chắn muốn xóa điểm này?')) {
      return;
    }

    this.loading = true;
    this.gradeService.deleteGrade(gradeDetail.rowNumber).subscribe({
      next: () => {
        gradeDetail.rowNumber = 0;
        gradeDetail.score = null;
        gradeDetail.isComment = false;
        gradeDetail.comment = null;
        this.showSuccess('Xóa điểm thành công');
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể xóa điểm');
        this.loading = false;
      }
    });
  }

  isEditing(studentId: string, gradeTypeId: string): boolean {
    return this.editingCell?.studentId === studentId &&
           this.editingCell?.gradeTypeId === gradeTypeId;
  }

  showError(message: string): void {
    this.errorMessage = message;
    this.successMessage = '';
    setTimeout(() => this.errorMessage = '', 5000);
  }

  showSuccess(message: string): void {
    this.successMessage = message;
    this.errorMessage = '';
    setTimeout(() => this.successMessage = '', 5000);
  }

  getClassSubjectKey(item: TeacherClassSubjectDto): string {
    return `${item.classId}|${item.subjectId}|${item.schoolYearId}`;
  }

  getGradeForStudent(student: StudentGradeDto, gradeTypeId: string): GradeDetailDto | undefined {
    return student.grades.find(g => g.gradeTypeId === gradeTypeId);
  }

  calculateAverage(student: StudentGradeDto): string {
    const grades = student.grades.filter(g => g.rowNumber > 0);

    if (grades.length === 0) {
      return '-';
    }

    // Check if all grades are in comment mode
    const allComments = grades.every(g => g.isComment);

    if (allComments) {
      // For comments: Pass if all are Pass, Fail if any is Fail
      const allPass = grades.every(g => g.comment === 'Pass');
      return allPass ? 'Pass' : 'Fail';
    }

    // Check if all grades are in score mode
    const allScores = grades.every(g => !g.isComment && g.score !== null);

    if (allScores) {
      // Calculate weighted average
      let totalWeightedScore = 0;
      let totalCoefficient = 0;

      for (const grade of grades) {
        if (grade.score !== null) {
          totalWeightedScore += grade.score * grade.coefficient;
          totalCoefficient += grade.coefficient;
        }
      }

      if (totalCoefficient === 0) {
        return '-';
      }

      const average = totalWeightedScore / totalCoefficient;
      return average.toFixed(2);
    }

    // Mixed mode or incomplete data
    return '-';
  }

  openImportDialog(): void {
    if (!this.selectedClass || !this.selectedSemester) {
      this.showError('Vui lòng chọn lớp, môn học và học kỳ trước khi import');
      return;
    }
    this.showImportDialog = true;
    this.importFile = null;
    this.importScoreType = 'score';
  }

  closeImportDialog(): void {
    this.showImportDialog = false;
    this.importFile = null;
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.importFile = input.files[0];
    }
  }

  async importGrades(): Promise<void> {
    if (!this.importFile || !this.selectedClass || !this.selectedSemester) {
      this.showError('Vui lòng chọn file để import');
      return;
    }

    try {
      this.loading = true;
      const formData = new FormData();
      formData.append('file', this.importFile);
      formData.append('classId', this.selectedClass.classId);
      formData.append('subjectId', this.selectedClass.subjectId);
      formData.append('semesterId', this.selectedSemester.semesterId);
      formData.append('schoolYearId', this.selectedClass.schoolYearId);
      formData.append('isComment', this.importScoreType === 'comment' ? 'true' : 'false');

      this.gradeService.importGradesFromExcel(formData).subscribe({
        next: (result: any) => {
          this.showSuccess(`Import thành công ${result.successCount} điểm`);
          this.closeImportDialog();
          this.loadStudentGrades();
          this.loading = false;
        },
        error: (error) => {
          this.showError(error.error?.message || 'Không thể import điểm');
          this.loading = false;
        }
      });
    } catch (error) {
      this.showError('Lỗi khi đọc file Excel');
      this.loading = false;
    }
  }

  openExportDialog(): void {
    if (!this.selectedClass || !this.selectedSemester) {
      this.showError('Vui lòng chọn lớp, môn học và học kỳ trước khi export');
      return;
    }
    this.showExportDialog = true;
    this.exportFormat = '';
  }

  closeExportDialog(): void {
    this.showExportDialog = false;
    this.exportFormat = '';
  }

  exportGrades(): void {
    if (!this.exportFormat || !this.selectedClass || !this.selectedSemester) {
      this.showError('Vui lòng chọn định dạng file');
      return;
    }

    this.loading = true;
    this.gradeService.exportGrades(
      this.selectedClass.classId,
      this.selectedClass.subjectId,
      this.selectedSemester.semesterId,
      this.selectedClass.schoolYearId,
      this.exportFormat
    ).subscribe({
      next: (blob: Blob) => {
        // Create download link
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;

        // Set filename based on format
        const extension = this.exportFormat === 'excel' ? 'xlsx' : this.exportFormat === 'word' ? 'docx' : 'pdf';
        const filename = `Diem_${this.selectedClass?.classId}_${this.selectedClass?.subjectId}_${this.selectedSemester?.semesterId}.${extension}`;
        link.download = filename;

        // Trigger download
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);

        this.showSuccess('Export thành công');
        this.closeExportDialog();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể export điểm');
        this.loading = false;
      }
    });
  }

  // Pagination methods
  get filteredStudents(): StudentGradeDto[] {
    if (!this.searchQuery.trim()) {
      return this.studentGrades;
    }

    const query = this.searchQuery.toLowerCase().trim();
    return this.studentGrades.filter(student =>
      student.studentId.toLowerCase().includes(query) ||
      student.studentName.toLowerCase().includes(query)
    );
  }

  get paginatedStudents(): StudentGradeDto[] {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    return this.filteredStudents.slice(startIndex, endIndex);
  }

  get startIndex(): number {
    return (this.currentPage - 1) * this.pageSize;
  }

  get endIndex(): number {
    return Math.min(this.startIndex + this.pageSize, this.filteredStudents.length);
  }

  updateTotalPages(): void {
    this.totalPages = Math.ceil(this.filteredStudents.length / this.pageSize);
    if (this.totalPages === 0) {
      this.totalPages = 1;
    }
  }

  onSearchChange(): void {
    this.currentPage = 1; // Reset to first page when search changes
    this.updateTotalPages();
  }

  clearSearch(): void {
    this.searchQuery = '';
    this.onSearchChange();
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  onPageSizeChange(event: Event): void {
    const select = event.target as HTMLSelectElement;
    this.pageSize = parseInt(select.value, 10);
    this.currentPage = 1;
    this.updateTotalPages();
  }

  getPageNumbers(): number[] {
    const pages: number[] = [];
    const maxPagesToShow = 5;

    if (this.totalPages <= maxPagesToShow) {
      // Show all pages if total pages is less than max
      for (let i = 1; i <= this.totalPages; i++) {
        pages.push(i);
      }
    } else {
      // Show current page and surrounding pages
      let startPage = Math.max(1, this.currentPage - 2);
      let endPage = Math.min(this.totalPages, this.currentPage + 2);

      // Adjust if at the beginning or end
      if (this.currentPage <= 3) {
        endPage = maxPagesToShow;
      } else if (this.currentPage >= this.totalPages - 2) {
        startPage = this.totalPages - maxPagesToShow + 1;
      }

      for (let i = startPage; i <= endPage; i++) {
        pages.push(i);
      }
    }

    return pages;
  }
}
