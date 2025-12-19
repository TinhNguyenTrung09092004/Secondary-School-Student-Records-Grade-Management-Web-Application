import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ResultService } from '../../../services/result.service';
import { ResultDto, CreateResultDto, UpdateResultDto } from '../../../models/result.model';

@Component({
  selector: 'app-result-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './result-management.component.html',
  styleUrl: './result-management.component.css'
})
export class ResultManagementComponent implements OnInit {
  private service = inject(ResultService);

  results: ResultDto[] = [];
  isEditing = false;
  editingId: string | null = null;
  showForm = false;

  formData = {
    resultId: '',
    resultName: ''
  };

  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.loadAll();
  }

  loadAll(): void {
    this.service.getAll().subscribe({
      next: (data) => {
        this.results = data;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách kết quả');
      }
    });
  }

  startAdd(): void {
    this.isEditing = false;
    this.editingId = null;
    this.showForm = true;
    this.resetForm();
  }

  startEdit(item: ResultDto): void {
    this.isEditing = true;
    this.editingId = item.resultId;
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
      const updateDto: UpdateResultDto = {
        resultName: this.formData.resultName
      };

      this.service.update(this.editingId, updateDto).subscribe({
        next: () => {
          this.showSuccess('Cập nhật kết quả thành công');
          this.loadAll();
          this.cancel();
        },
        error: (error) => {
          this.showError(error.error?.message || 'Không thể cập nhật kết quả');
        }
      });
    } else {
      const createDto: CreateResultDto = { ...this.formData };

      this.service.create(createDto).subscribe({
        next: () => {
          this.showSuccess('Thêm kết quả thành công');
          this.loadAll();
          this.cancel();
        },
        error: (error) => {
          this.showError(error.error?.message || 'Không thể thêm kết quả');
        }
      });
    }
  }

  delete(id: string): void {
    if (confirm('Bạn có chắc chắn muốn xóa kết quả này?')) {
      this.service.delete(id).subscribe({
        next: () => {
          this.showSuccess('Xóa kết quả thành công');
          this.loadAll();
        },
        error: (error) => {
          this.showError(error.error?.message || 'Không thể xóa kết quả');
        }
      });
    }
  }

  resetForm(): void {
    this.formData = {
      resultId: '',
      resultName: ''
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