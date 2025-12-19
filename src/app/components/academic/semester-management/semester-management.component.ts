import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SemesterService } from '../../../services/semester.service';
import { SemesterDto, CreateSemesterDto, UpdateSemesterDto } from '../../../models/semester.model';

@Component({
  selector: 'app-semester-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './semester-management.component.html',
  styleUrls: ['./semester-management.component.css']
})
export class SemesterManagementComponent implements OnInit {
  private semesterService = inject(SemesterService);

  semesterList: SemesterDto[] = [];
  selectedSemester: SemesterDto | null = null;
  showCreateModal = false;
  showEditModal = false;
  loading = false;
  errorMessage = '';
  successMessage = '';

  newSemester: CreateSemesterDto = {
    semesterId: '',
    semesterName: '',
    coefficient: 1
  };

  editSemester: UpdateSemesterDto = {
    semesterId: '',
    semesterName: '',
    coefficient: 1
  };

  ngOnInit(): void {
    this.loadSemesters();
  }

  loadSemesters(): void {
    this.loading = true;
    this.semesterService.getAllSemesters().subscribe({
      next: (data) => {
        this.semesterList = data;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách học kỳ');
        this.loading = false;
      }
    });
  }

  openCreateModal(): void {
    this.newSemester = {
      semesterId: '',
      semesterName: '',
      coefficient: 1
    };
    this.showCreateModal = true;
    this.clearMessages();
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  createSemester(): void {
    if (!this.newSemester.semesterId || !this.newSemester.semesterName || this.newSemester.coefficient <= 0) {
      this.showError('Vui lòng điền đầy đủ thông tin hợp lệ');
      return;
    }

    this.loading = true;
    this.semesterService.createSemester(this.newSemester).subscribe({
      next: (data) => {
        this.showSuccess('Thêm học kỳ thành công');
        this.loadSemesters();
        this.closeCreateModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể thêm học kỳ');
        this.loading = false;
      }
    });
  }

  openEditModal(semester: SemesterDto): void {
    this.selectedSemester = semester;
    this.editSemester = {
      semesterId: semester.semesterId,
      semesterName: semester.semesterName,
      coefficient: semester.coefficient
    };
    this.showEditModal = true;
    this.clearMessages();
  }

  closeEditModal(): void {
    this.showEditModal = false;
    this.selectedSemester = null;
  }

  updateSemester(): void {
    if (!this.selectedSemester || !this.editSemester.semesterId || !this.editSemester.semesterName || this.editSemester.coefficient <= 0) {
      this.showError('Vui lòng điền đầy đủ thông tin hợp lệ');
      return;
    }

    this.loading = true;
    this.semesterService.updateSemester(this.selectedSemester.semesterId, this.editSemester).subscribe({
      next: (data) => {
        this.showSuccess('Cập nhật học kỳ thành công');
        this.loadSemesters();
        this.closeEditModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể cập nhật học kỳ');
        this.loading = false;
      }
    });
  }

  deleteSemester(semester: SemesterDto): void {
    if (!confirm(`Bạn có chắc chắn muốn xóa học kỳ "${semester.semesterName}"?`)) {
      return;
    }

    this.loading = true;
    this.semesterService.deleteSemester(semester.semesterId).subscribe({
      next: () => {
        this.showSuccess('Xóa học kỳ thành công');
        this.loadSemesters();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể xóa học kỳ');
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
