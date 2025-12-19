import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SchoolYearService } from '../../../services/schoolyear.service';
import { SchoolYearDto, CreateSchoolYearDto, UpdateSchoolYearDto } from '../../../models/schoolyear.model';

@Component({
  selector: 'app-school-year-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './school-year-management.component.html',
  styleUrls: ['./school-year-management.component.css']
})
export class SchoolYearManagementComponent implements OnInit {
  private schoolYearService = inject(SchoolYearService);

  schoolYearList: SchoolYearDto[] = [];
  selectedSchoolYear: SchoolYearDto | null = null;
  showCreateModal = false;
  showEditModal = false;
  loading = false;
  errorMessage = '';
  successMessage = '';

  newSchoolYear: CreateSchoolYearDto = {
    schoolYearId: '',
    schoolYearName: ''
  };

  editSchoolYear: UpdateSchoolYearDto = {
    schoolYearId: '',
    schoolYearName: ''
  };

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading = true;
    this.schoolYearService.getAllSchoolYears().subscribe({
      next: (data) => {
        this.schoolYearList = data;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải dữ liệu');
        this.loading = false;
      }
    });
  }

  openCreateModal(): void {
    this.newSchoolYear = {
      schoolYearId: '',
      schoolYearName: ''
    };
    this.showCreateModal = true;
    this.clearMessages();
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  createSchoolYear(): void {
    if (!this.newSchoolYear.schoolYearId || !this.newSchoolYear.schoolYearName) {
      this.showError('Vui lòng điền đầy đủ thông tin');
      return;
    }

    this.loading = true;
    this.schoolYearService.createSchoolYear(this.newSchoolYear).subscribe({
      next: (data) => {
        this.showSuccess('Thêm năm học thành công');
        this.loadData();
        this.closeCreateModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể thêm năm học');
        this.loading = false;
      }
    });
  }

  openEditModal(schoolYear: SchoolYearDto): void {
    this.selectedSchoolYear = schoolYear;
    this.editSchoolYear = {
      schoolYearId: schoolYear.schoolYearId,
      schoolYearName: schoolYear.schoolYearName
    };
    this.showEditModal = true;
    this.clearMessages();
  }

  closeEditModal(): void {
    this.showEditModal = false;
    this.selectedSchoolYear = null;
  }

  updateSchoolYear(): void {
    if (!this.selectedSchoolYear || !this.editSchoolYear.schoolYearId || !this.editSchoolYear.schoolYearName) {
      this.showError('Vui lòng điền đầy đủ thông tin');
      return;
    }

    this.loading = true;
    this.schoolYearService.updateSchoolYear(this.selectedSchoolYear.schoolYearId, this.editSchoolYear).subscribe({
      next: (data) => {
        this.showSuccess('Cập nhật năm học thành công');
        this.loadData();
        this.closeEditModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể cập nhật năm học');
        this.loading = false;
      }
    });
  }

  deleteSchoolYear(schoolYear: SchoolYearDto): void {
    if (!confirm(`Bạn có chắc chắn muốn xóa năm học "${schoolYear.schoolYearName}"?`)) {
      return;
    }

    this.loading = true;
    this.schoolYearService.deleteSchoolYear(schoolYear.schoolYearId).subscribe({
      next: () => {
        this.showSuccess('Xóa năm học thành công');
        this.loadData();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể xóa năm học');
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
