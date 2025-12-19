import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EthnicityService } from '../../../services/ethnicity.service';
import { EthnicityDto, CreateEthnicityDto, UpdateEthnicityDto } from '../../../models/ethnicity.model';

@Component({
  selector: 'app-ethnicity-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ethnicity-management.component.html'
})
export class EthnicityManagementComponent implements OnInit {
  private ethnicityService = inject(EthnicityService);

  ethnicityList: EthnicityDto[] = [];
  selectedEthnicity: EthnicityDto | null = null;
  showCreateModal = false;
  showEditModal = false;
  loading = false;
  errorMessage = '';
  successMessage = '';

  newEthnicity: CreateEthnicityDto = {
    ethnicityName: ''
  };

  editEthnicity: UpdateEthnicityDto = {
    ethnicityName: ''
  };

  ngOnInit(): void {
    this.loadEthnicity();
  }

  loadEthnicity(): void {
    this.loading = true;
    this.ethnicityService.getAllEthnicity().subscribe({
      next: (data) => {
        this.ethnicityList = data;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách dân tộc');
        this.loading = false;
      }
    });
  }

  openCreateModal(): void {
    this.newEthnicity = { ethnicityName: '' };
    this.showCreateModal = true;
    this.clearMessages();
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  createEthnicity(): void {
    if (!this.newEthnicity.ethnicityName) {
      this.showError('Vui lòng nhập tên dân tộc');
      return;
    }

    this.loading = true;
    this.ethnicityService.createEthnicity(this.newEthnicity).subscribe({
      next: (data) => {
        this.showSuccess('Thêm dân tộc thành công');
        this.loadEthnicity();
        this.closeCreateModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể thêm dân tộc');
        this.loading = false;
      }
    });
  }

  openEditModal(ethnicity: EthnicityDto): void {
    this.selectedEthnicity = ethnicity;
    this.editEthnicity = { ethnicityName: ethnicity.ethnicityName };
    this.showEditModal = true;
    this.clearMessages();
  }

  closeEditModal(): void {
    this.showEditModal = false;
    this.selectedEthnicity = null;
  }

  updateEthnicity(): void {
    if (!this.selectedEthnicity || !this.editEthnicity.ethnicityName) {
      this.showError('Vui lòng điền đầy đủ thông tin');
      return;
    }

    this.loading = true;
    this.ethnicityService.updateEthnicity(this.selectedEthnicity.ethnicityId, this.editEthnicity).subscribe({
      next: (data) => {
        this.showSuccess('Cập nhật dân tộc thành công');
        this.loadEthnicity();
        this.closeEditModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể cập nhật dân tộc');
        this.loading = false;
      }
    });
  }

  deleteEthnicity(ethnicity: EthnicityDto): void {
    if (!confirm(`Bạn có chắc chắn muốn xóa dân tộc "${ethnicity.ethnicityName}"?`)) {
      return;
    }

    this.loading = true;
    this.ethnicityService.deleteEthnicity(ethnicity.ethnicityId).subscribe({
      next: () => {
        this.showSuccess('Xóa dân tộc thành công');
        this.loadEthnicity();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể xóa dân tộc');
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
