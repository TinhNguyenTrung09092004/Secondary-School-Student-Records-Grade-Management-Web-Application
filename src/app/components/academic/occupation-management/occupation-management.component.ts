import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OccupationService } from '../../../services/occupation.service';
import { OccupationDto, CreateOccupationDto, UpdateOccupationDto } from '../../../models/occupation.model';

@Component({
  selector: 'app-occupation-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './occupation-management.component.html',
  styleUrls: ['./occupation-management.component.css']
})
export class OccupationManagementComponent implements OnInit {
  private occupationService = inject(OccupationService);

  occupationList: OccupationDto[] = [];
  selectedOccupation: OccupationDto | null = null;
  showCreateModal = false;
  showEditModal = false;
  loading = false;
  errorMessage = '';
  successMessage = '';

  newOccupation: CreateOccupationDto = {
    occupationName: ''
  };

  editOccupation: UpdateOccupationDto = {
    occupationName: ''
  };

  ngOnInit(): void {
    this.loadOccupation();
  }

  loadOccupation(): void {
    this.loading = true;
    this.occupationService.getAllOccupation().subscribe({
      next: (data) => {
        this.occupationList = data;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách nghề nghiệp');
        this.loading = false;
      }
    });
  }

  openCreateModal(): void {
    this.newOccupation = { occupationName: '' };
    this.showCreateModal = true;
    this.clearMessages();
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  createOccupation(): void {
    if (!this.newOccupation.occupationName) {
      this.showError('Vui lòng nhập tên nghề nghiệp');
      return;
    }

    this.loading = true;
    this.occupationService.createOccupation(this.newOccupation).subscribe({
      next: (data) => {
        this.showSuccess('Thêm nghề nghiệp thành công');
        this.loadOccupation();
        this.closeCreateModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể thêm nghề nghiệp');
        this.loading = false;
      }
    });
  }

  openEditModal(occupation: OccupationDto): void {
    this.selectedOccupation = occupation;
    this.editOccupation = { occupationName: occupation.occupationName };
    this.showEditModal = true;
    this.clearMessages();
  }

  closeEditModal(): void {
    this.showEditModal = false;
    this.selectedOccupation = null;
  }

  updateOccupation(): void {
    if (!this.selectedOccupation || !this.editOccupation.occupationName) {
      this.showError('Vui lòng điền đầy đủ thông tin');
      return;
    }

    this.loading = true;
    this.occupationService.updateOccupation(this.selectedOccupation.occupationId, this.editOccupation).subscribe({
      next: (data) => {
        this.showSuccess('Cập nhật nghề nghiệp thành công');
        this.loadOccupation();
        this.closeEditModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể cập nhật nghề nghiệp');
        this.loading = false;
      }
    });
  }

  deleteOccupation(occupation: OccupationDto): void {
    if (!confirm(`Bạn có chắc chắn muốn xóa nghề nghiệp "${occupation.occupationName}"?`)) {
      return;
    }

    this.loading = true;
    this.occupationService.deleteOccupation(occupation.occupationId).subscribe({
      next: () => {
        this.showSuccess('Xóa nghề nghiệp thành công');
        this.loadOccupation();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể xóa nghề nghiệp');
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
