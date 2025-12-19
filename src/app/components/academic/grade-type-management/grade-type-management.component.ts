import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GradeService } from '../../../services/grade.service';
import { GradeTypeDto, CreateGradeTypeDto, UpdateGradeTypeDto } from '../../../models/grade.model';

@Component({
  selector: 'app-grade-type-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './grade-type-management.component.html',
  styleUrls: ['./grade-type-management.component.css']
})
export class GradeTypeManagementComponent implements OnInit {
  private gradeService = inject(GradeService);

  gradeTypeList: GradeTypeDto[] = [];
  selectedGradeType: GradeTypeDto | null = null;
  showCreateModal = false;
  showEditModal = false;
  loading = false;
  errorMessage = '';
  successMessage = '';

  newGradeType: CreateGradeTypeDto = {
    gradeTypeId: '',
    gradeTypeName: '',
    coefficient: 1
  };

  editGradeType: UpdateGradeTypeDto = {
    gradeTypeId: '',
    gradeTypeName: '',
    coefficient: 1
  };

  ngOnInit(): void {
    this.loadGradeTypes();
  }

  loadGradeTypes(): void {
    this.loading = true;
    this.gradeService.getGradeTypes().subscribe({
      next: (data) => {
        this.gradeTypeList = data;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách loại điểm');
        this.loading = false;
      }
    });
  }

  openCreateModal(): void {
    this.newGradeType = {
      gradeTypeId: '',
      gradeTypeName: '',
      coefficient: 1
    };
    this.showCreateModal = true;
    this.clearMessages();
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  createGradeType(): void {
    if (!this.newGradeType.gradeTypeId || !this.newGradeType.gradeTypeName || this.newGradeType.coefficient <= 0) {
      this.showError('Vui lòng điền đầy đủ thông tin hợp lệ');
      return;
    }

    this.loading = true;
    this.gradeService.createGradeType(this.newGradeType).subscribe({
      next: (data) => {
        this.showSuccess('Thêm loại điểm thành công');
        this.loadGradeTypes();
        this.closeCreateModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể thêm loại điểm');
        this.loading = false;
      }
    });
  }

  openEditModal(gradeType: GradeTypeDto): void {
    this.selectedGradeType = gradeType;
    this.editGradeType = {
      gradeTypeId: gradeType.gradeTypeId,
      gradeTypeName: gradeType.gradeTypeName,
      coefficient: gradeType.coefficient
    };
    this.showEditModal = true;
    this.clearMessages();
  }

  closeEditModal(): void {
    this.showEditModal = false;
    this.selectedGradeType = null;
  }

  updateGradeType(): void {
    if (!this.selectedGradeType || !this.editGradeType.gradeTypeId || !this.editGradeType.gradeTypeName || this.editGradeType.coefficient <= 0) {
      this.showError('Vui lòng điền đầy đủ thông tin hợp lệ');
      return;
    }

    this.loading = true;
    this.gradeService.updateGradeType(this.selectedGradeType.gradeTypeId, this.editGradeType).subscribe({
      next: (data) => {
        this.showSuccess('Cập nhật loại điểm thành công');
        this.loadGradeTypes();
        this.closeEditModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể cập nhật loại điểm');
        this.loading = false;
      }
    });
  }

  deleteGradeType(gradeType: GradeTypeDto): void {
    if (!confirm(`Bạn có chắc chắn muốn xóa loại điểm "${gradeType.gradeTypeName}"?`)) {
      return;
    }

    this.loading = true;
    this.gradeService.deleteGradeType(gradeType.gradeTypeId).subscribe({
      next: () => {
        this.showSuccess('Xóa loại điểm thành công');
        this.loadGradeTypes();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể xóa loại điểm');
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
