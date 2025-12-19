import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GradeLevelService } from '../../../services/gradelevel.service';
import { GradeLevelDto, CreateGradeLevelDto, UpdateGradeLevelDto } from '../../../models/gradelevel.model';

@Component({
  selector: 'app-grade-level-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './grade-level-management.component.html',
  styleUrls: ['./grade-level-management.component.css']
})
export class GradeLevelManagementComponent implements OnInit {
  private gradeLevelService = inject(GradeLevelService);

  gradeLevelList: GradeLevelDto[] = [];
  selectedGradeLevel: GradeLevelDto | null = null;
  showCreateModal = false;
  showEditModal = false;
  loading = false;
  errorMessage = '';
  successMessage = '';

  newGradeLevel: CreateGradeLevelDto = {
    gradeLevelId: '',
    gradeLevelName: ''
  };

  editGradeLevel: UpdateGradeLevelDto = {
    gradeLevelId: '',
    gradeLevelName: ''
  };

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading = true;
    this.gradeLevelService.getAllGradeLevels().subscribe({
      next: (data) => {
        this.gradeLevelList = data;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải dữ liệu');
        this.loading = false;
      }
    });
  }

  openCreateModal(): void {
    this.newGradeLevel = {
      gradeLevelId: '',
      gradeLevelName: ''
    };
    this.showCreateModal = true;
    this.clearMessages();
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  createGradeLevel(): void {
    if (!this.newGradeLevel.gradeLevelId || !this.newGradeLevel.gradeLevelName) {
      this.showError('Vui lòng điền đầy đủ thông tin');
      return;
    }

    this.loading = true;
    this.gradeLevelService.createGradeLevel(this.newGradeLevel).subscribe({
      next: (data) => {
        this.showSuccess('Thêm khối thành công');
        this.loadData();
        this.closeCreateModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể thêm khối');
        this.loading = false;
      }
    });
  }

  openEditModal(gradeLevel: GradeLevelDto): void {
    this.selectedGradeLevel = gradeLevel;
    this.editGradeLevel = {
      gradeLevelId: gradeLevel.gradeLevelId,
      gradeLevelName: gradeLevel.gradeLevelName
    };
    this.showEditModal = true;
    this.clearMessages();
  }

  closeEditModal(): void {
    this.showEditModal = false;
    this.selectedGradeLevel = null;
  }

  updateGradeLevel(): void {
    if (!this.selectedGradeLevel || !this.editGradeLevel.gradeLevelId || !this.editGradeLevel.gradeLevelName) {
      this.showError('Vui lòng điền đầy đủ thông tin');
      return;
    }

    this.loading = true;
    this.gradeLevelService.updateGradeLevel(this.selectedGradeLevel.gradeLevelId, this.editGradeLevel).subscribe({
      next: (data) => {
        this.showSuccess('Cập nhật khối thành công');
        this.loadData();
        this.closeEditModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể cập nhật khối');
        this.loading = false;
      }
    });
  }

  deleteGradeLevel(gradeLevel: GradeLevelDto): void {
    if (!confirm(`Bạn có chắc chắn muốn xóa khối "${gradeLevel.gradeLevelName}"?`)) {
      return;
    }

    this.loading = true;
    this.gradeLevelService.deleteGradeLevel(gradeLevel.gradeLevelId).subscribe({
      next: () => {
        this.showSuccess('Xóa khối thành công');
        this.loadData();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể xóa khối');
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
