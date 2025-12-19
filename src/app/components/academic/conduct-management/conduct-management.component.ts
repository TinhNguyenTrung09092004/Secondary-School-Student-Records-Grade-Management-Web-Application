import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ConductService } from '../../../services/conduct.service';
import { ConductDto, CreateConductDto, UpdateConductDto } from '../../../models/conduct.model';

@Component({
  selector: 'app-conduct-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './conduct-management.component.html',
  styleUrls: ['./conduct-management.component.css']
})
export class ConductManagementComponent implements OnInit {
  private conductService = inject(ConductService);

  conductList: ConductDto[] = [];
  selectedConduct: ConductDto | null = null;
  showCreateModal = false;
  showEditModal = false;
  loading = false;
  errorMessage = '';
  successMessage = '';

  newConduct: CreateConductDto = {
    conductName: ''
  };

  editConduct: UpdateConductDto = {
    conductName: ''
  };

  ngOnInit(): void {
    this.loadConduct();
  }

  loadConduct(): void {
    this.loading = true;
    this.conductService.getAllConduct().subscribe({
      next: (data) => {
        this.conductList = data;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách hạnh kiểm');
        this.loading = false;
      }
    });
  }

  openCreateModal(): void {
    this.newConduct = { conductName: '' };
    this.showCreateModal = true;
    this.clearMessages();
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  createConduct(): void {
    if (!this.newConduct.conductName) {
      this.showError('Vui lòng nhập tên hạnh kiểm');
      return;
    }

    this.loading = true;
    this.conductService.createConduct(this.newConduct).subscribe({
      next: (data) => {
        this.showSuccess('Thêm hạnh kiểm thành công');
        this.loadConduct();
        this.closeCreateModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể thêm hạnh kiểm');
        this.loading = false;
      }
    });
  }

  openEditModal(conduct: ConductDto): void {
    this.selectedConduct = conduct;
    this.editConduct = { conductName: conduct.conductName };
    this.showEditModal = true;
    this.clearMessages();
  }

  closeEditModal(): void {
    this.showEditModal = false;
    this.selectedConduct = null;
  }

  updateConduct(): void {
    if (!this.selectedConduct || !this.editConduct.conductName) {
      this.showError('Vui lòng điền đầy đủ thông tin');
      return;
    }

    this.loading = true;
    this.conductService.updateConduct(this.selectedConduct.conductId, this.editConduct).subscribe({
      next: (data) => {
        this.showSuccess('Cập nhật hạnh kiểm thành công');
        this.loadConduct();
        this.closeEditModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể cập nhật hạnh kiểm');
        this.loading = false;
      }
    });
  }

  deleteConduct(conduct: ConductDto): void {
    if (!confirm(`Bạn có chắc chắn muốn xóa hạnh kiểm "${conduct.conductName}"?`)) {
      return;
    }

    this.loading = true;
    this.conductService.deleteConduct(conduct.conductId).subscribe({
      next: () => {
        this.showSuccess('Xóa hạnh kiểm thành công');
        this.loadConduct();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể xóa hạnh kiểm');
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
