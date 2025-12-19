import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SubjectService } from '../../../services/subject.service';
import { SubjectDto, CreateSubjectDto, UpdateSubjectDto } from '../../../models/subject.model';

@Component({
  selector: 'app-subject-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './subject-management.component.html',
  styleUrls: ['./subject-management.component.css']
})
export class SubjectManagementComponent implements OnInit {
  private subjectService = inject(SubjectService);

  subjectList: SubjectDto[] = [];
  selectedSubject: SubjectDto | null = null;
  showCreateModal = false;
  showEditModal = false;
  loading = false;
  errorMessage = '';
  successMessage = '';

  newSubject: CreateSubjectDto = {
    subjectName: '',
    lessonCount: 0,
    coefficient: 0
  };

  editSubject: UpdateSubjectDto = {
    subjectName: '',
    lessonCount: 0,
    coefficient: 0
  };

  ngOnInit(): void {
    this.loadSubjects();
  }

  loadSubjects(): void {
    this.loading = true;
    this.subjectService.getAllSubjects().subscribe({
      next: (data) => {
        this.subjectList = data;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách môn học');
        this.loading = false;
      }
    });
  }

  openCreateModal(): void {
    this.newSubject = {
      subjectName: '',
      lessonCount: 0,
      coefficient: 0
    };
    this.showCreateModal = true;
    this.clearMessages();
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  createSubject(): void {
    if (!this.newSubject.subjectName || this.newSubject.lessonCount <= 0 || this.newSubject.coefficient <= 0) {
      this.showError('Vui lòng điền đầy đủ thông tin hợp lệ');
      return;
    }

    this.loading = true;
    this.subjectService.createSubject(this.newSubject).subscribe({
      next: (data) => {
        this.showSuccess('Thêm môn học thành công');
        this.loadSubjects();
        this.closeCreateModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể thêm môn học');
        this.loading = false;
      }
    });
  }

  openEditModal(subject: SubjectDto): void {
    this.selectedSubject = subject;
    this.editSubject = {
      subjectName: subject.subjectName,
      lessonCount: subject.lessonCount,
      coefficient: subject.coefficient
    };
    this.showEditModal = true;
    this.clearMessages();
  }

  closeEditModal(): void {
    this.showEditModal = false;
    this.selectedSubject = null;
  }

  updateSubject(): void {
    if (!this.selectedSubject || !this.editSubject.subjectName || this.editSubject.lessonCount <= 0 || this.editSubject.coefficient <= 0) {
      this.showError('Vui lòng điền đầy đủ thông tin hợp lệ');
      return;
    }

    this.loading = true;
    this.subjectService.updateSubject(this.selectedSubject.subjectId, this.editSubject).subscribe({
      next: (data) => {
        this.showSuccess('Cập nhật môn học thành công');
        this.loadSubjects();
        this.closeEditModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể cập nhật môn học');
        this.loading = false;
      }
    });
  }

  deleteSubject(subject: SubjectDto): void {
    if (!confirm(`Bạn có chắc chắn muốn xóa môn học "${subject.subjectName}"?`)) {
      return;
    }

    this.loading = true;
    this.subjectService.deleteSubject(subject.subjectId).subscribe({
      next: () => {
        this.showSuccess('Xóa môn học thành công');
        this.loadSubjects();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể xóa môn học');
        this.loading = false;
      }
    });
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

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }
}
