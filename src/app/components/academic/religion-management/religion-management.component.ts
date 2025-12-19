import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReligionService } from '../../../services/religion.service';
import { ReligionDto, CreateReligionDto, UpdateReligionDto } from '../../../models/religion.model';

@Component({
  selector: 'app-religion-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './religion-management.component.html',
  styleUrls: ['./religion-management.component.css']
})
export class ReligionManagementComponent implements OnInit {
  private religionService = inject(ReligionService);

  religionList: ReligionDto[] = [];
  selectedReligion: ReligionDto | null = null;
  showCreateModal = false;
  showEditModal = false;
  loading = false;
  errorMessage = '';
  successMessage = '';

  newReligion: CreateReligionDto = {
    religionName: ''
  };

  editReligion: UpdateReligionDto = {
    religionName: ''
  };

  ngOnInit(): void {
    this.loadReligion();
  }

  loadReligion(): void {
    this.loading = true;
    this.religionService.getAllReligion().subscribe({
      next: (data) => {
        this.religionList = data;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách tôn giáo');
        this.loading = false;
      }
    });
  }

  openCreateModal(): void {
    this.newReligion = { religionName: '' };
    this.showCreateModal = true;
    this.clearMessages();
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  createReligion(): void {
    if (!this.newReligion.religionName) {
      this.showError('Vui lòng nhập tên tôn giáo');
      return;
    }

    this.loading = true;
    this.religionService.createReligion(this.newReligion).subscribe({
      next: (data) => {
        this.showSuccess('Thêm tôn giáo thành công');
        this.loadReligion();
        this.closeCreateModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể thêm tôn giáo');
        this.loading = false;
      }
    });
  }

  openEditModal(religion: ReligionDto): void {
    this.selectedReligion = religion;
    this.editReligion = { religionName: religion.religionName };
    this.showEditModal = true;
    this.clearMessages();
  }

  closeEditModal(): void {
    this.showEditModal = false;
    this.selectedReligion = null;
  }

  updateReligion(): void {
    if (!this.selectedReligion || !this.editReligion.religionName) {
      this.showError('Vui lòng điền đầy đủ thông tin');
      return;
    }

    this.loading = true;
    this.religionService.updateReligion(this.selectedReligion.religionId, this.editReligion).subscribe({
      next: (data) => {
        this.showSuccess('Cập nhật tôn giáo thành công');
        this.loadReligion();
        this.closeEditModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể cập nhật tôn giáo');
        this.loading = false;
      }
    });
  }

  deleteReligion(religion: ReligionDto): void {
    if (!confirm(`Bạn có chắc chắn muốn xóa tôn giáo "${religion.religionName}"?`)) {
      return;
    }

    this.loading = true;
    this.religionService.deleteReligion(religion.religionId).subscribe({
      next: () => {
        this.showSuccess('Xóa tôn giáo thành công');
        this.loadReligion();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể xóa tôn giáo');
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
