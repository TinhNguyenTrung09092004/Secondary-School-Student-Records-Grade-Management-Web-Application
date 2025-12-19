import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AcademicPerformanceService } from '../../../services/academic-performance.service';
import { AcademicPerformanceDto, CreateAcademicPerformanceDto, UpdateAcademicPerformanceDto } from '../../../models/academic-performance.model';

@Component({
  selector: 'app-academic-performance-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './academic-performance-management.component.html',
  styleUrl: './academic-performance-management.component.css'
})
export class AcademicPerformanceManagementComponent implements OnInit {
  private service = inject(AcademicPerformanceService);

  academicPerformances: AcademicPerformanceDto[] = [];
  isEditing = false;
  editingId: string | null = null;
  showForm = false;

  formData = {
    academicPerformanceId: '',
    academicPerformanceName: ''
  };

  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.loadAll();
  }

  loadAll(): void {
    this.service.getAll().subscribe({
      next: (data) => {
        this.academicPerformances = data;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách học lực');
      }
    });
  }

  startAdd(): void {
    this.isEditing = false;
    this.editingId = null;
    this.showForm = true;
    this.resetForm();
  }

  startEdit(item: AcademicPerformanceDto): void {
    this.isEditing = true;
    this.editingId = item.academicPerformanceId;
    this.showForm = true;
    this.formData = { ...item };
  }

  cancel(): void {
    this.isEditing = false;
    this.editingId = null;
    this.showForm = false;
    this.resetForm();
  }

  save(): void {
    if (this.isEditing && this.editingId) {
      const updateDto: UpdateAcademicPerformanceDto = {
        academicPerformanceName: this.formData.academicPerformanceName
      };

      this.service.update(this.editingId, updateDto).subscribe({
        next: () => {
          this.showSuccess('Cập nhật học lực thành công');
          this.loadAll();
          this.cancel();
        },
        error: (error) => {
          this.showError(error.error?.message || 'Không thể cập nhật học lực');
        }
      });
    } else {
      const createDto: CreateAcademicPerformanceDto = { ...this.formData };

      this.service.create(createDto).subscribe({
        next: () => {
          this.showSuccess('Thêm học lực thành công');
          this.loadAll();
          this.cancel();
        },
        error: (error) => {
          this.showError(error.error?.message || 'Không thể thêm học lực');
        }
      });
    }
  }

  delete(id: string): void {
    if (confirm('Bạn có chắc chắn muốn xóa học lực này?')) {
      this.service.delete(id).subscribe({
        next: () => {
          this.showSuccess('Xóa học lực thành công');
          this.loadAll();
        },
        error: (error) => {
          this.showError(error.error?.message || 'Không thể xóa học lực');
        }
      });
    }
  }

  resetForm(): void {
    this.formData = {
      academicPerformanceId: '',
      academicPerformanceName: ''
    };
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
}