import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DepartmentService } from '../../../services/department.service';
import { DepartmentDto, CreateDepartmentDto, UpdateDepartmentDto } from '../../../models/department.model';
import { TeacherService } from '../../../services/teacher.service';
import { TeacherDto } from '../../../models/teacher.model';

interface Toast {
  message: string;
  type: 'success' | 'error';
  closing?: boolean;
}

@Component({
  selector: 'app-department-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './department-management.component.html',
  styleUrls: ['./department-management.component.css']
})
export class DepartmentManagementComponent implements OnInit {
  private departmentService = inject(DepartmentService);
  private teacherService = inject(TeacherService);

  departmentList: DepartmentDto[] = [];
  teacherList: TeacherDto[] = [];
  selectedDepartment: DepartmentDto | null = null;
  showCreateModal = false;
  showEditModal = false;
  loading = false;
  errorMessage = '';
  successMessage = '';
  toasts: Toast[] = [];

  newDepartment: CreateDepartmentDto = {
    departmentId: '',
    departmentName: '',
    headTeacherId: ''
  };

  editDepartment: UpdateDepartmentDto = {
    departmentName: '',
    headTeacherId: ''
  };

  ngOnInit(): void {
    this.loadDepartments();
    this.loadTeachers();
  }

  loadDepartments(): void {
    this.loading = true;
    this.departmentService.getAllDepartments().subscribe({
      next: (data) => {
        this.departmentList = data;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải dữ liệu');
        this.loading = false;
      }
    });
  }

  loadTeachers(): void {
    this.teacherService.getAllTeachers().subscribe({
      next: (data) => {
        this.teacherList = data;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách giáo viên');
      }
    });
  }

  openCreateModal(): void {
    this.newDepartment = {
      departmentId: '',
      departmentName: '',
      headTeacherId: ''
    };
    this.showCreateModal = true;
    this.clearMessages();
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  createDepartment(): void {
    if (!this.newDepartment.departmentId || !this.newDepartment.departmentName) {
      this.showError('Vui lòng điền đầy đủ thông tin bắt buộc');
      return;
    }

    this.loading = true;
    const departmentData = {
      ...this.newDepartment,
      headTeacherId: this.newDepartment.headTeacherId || undefined
    };

    this.departmentService.createDepartment(departmentData).subscribe({
      next: (data) => {
        this.showSuccess('Thêm tổ bộ môn thành công');
        this.loadDepartments();
        this.closeCreateModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể thêm tổ bộ môn');
        this.loading = false;
      }
    });
  }

  openEditModal(department: DepartmentDto): void {
    this.selectedDepartment = department;
    this.editDepartment = {
      departmentName: department.departmentName,
      headTeacherId: department.headTeacherId || ''
    };
    this.showEditModal = true;
    this.clearMessages();
  }

  closeEditModal(): void {
    this.showEditModal = false;
    this.selectedDepartment = null;
  }

  updateDepartment(): void {
    if (!this.selectedDepartment || !this.editDepartment.departmentName) {
      this.showError('Vui lòng điền đầy đủ thông tin bắt buộc');
      return;
    }

    this.loading = true;
    const departmentData = {
      ...this.editDepartment,
      headTeacherId: this.editDepartment.headTeacherId || undefined
    };

    this.departmentService.updateDepartment(this.selectedDepartment.departmentId, departmentData).subscribe({
      next: (data) => {
        this.showSuccess('Cập nhật tổ bộ môn thành công');
        this.loadDepartments();
        this.closeEditModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể cập nhật tổ bộ môn');
        this.loading = false;
      }
    });
  }

  deleteDepartment(department: DepartmentDto): void {
    if (!confirm(`Bạn có chắc chắn muốn xóa tổ bộ môn "${department.departmentName}"?`)) {
      return;
    }

    this.loading = true;
    this.departmentService.deleteDepartment(department.departmentId).subscribe({
      next: () => {
        this.showSuccess('Xóa tổ bộ môn thành công');
        this.loadDepartments();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể xóa tổ bộ môn');
        this.loading = false;
      }
    });
  }

  showError(message: string): void {
    this.errorMessage = message;
    this.successMessage = '';
    this.showToast(message, 'error');
  }

  showSuccess(message: string): void {
    this.successMessage = message;
    this.errorMessage = '';
    this.showToast(message, 'success');
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  showToast(message: string, type: 'success' | 'error'): void {
    const toast: Toast = { message, type };
    this.toasts.push(toast);

    setTimeout(() => {
      const index = this.toasts.indexOf(toast);
      if (index > -1) {
        this.toasts[index].closing = true;
        setTimeout(() => {
          this.toasts = this.toasts.filter(t => t !== toast);
        }, 300);
      }
    }, 5000);
  }

  removeToast(index: number): void {
    this.toasts[index].closing = true;
    setTimeout(() => {
      this.toasts.splice(index, 1);
    }, 300);
  }
}
