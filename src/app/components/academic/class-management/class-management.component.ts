import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClassService } from '../../../services/class.service';
import { GradeLevelService } from '../../../services/gradelevel.service';
import { SchoolYearService } from '../../../services/schoolyear.service';
import { AuthService } from '../../../services/auth.service';
import { ClassDto, CreateClassDto, UpdateClassDto } from '../../../models/class.model';
import { GradeLevelDto } from '../../../models/gradelevel.model';
import { SchoolYearDto } from '../../../models/schoolyear.model';
import { TeacherDto } from '../../../models/teacher.model';

interface Toast {
  message: string;
  type: 'success' | 'error';
  closing?: boolean;
}

@Component({
  selector: 'app-class-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './class-management.component.html',
  styleUrls: ['./class-management.component.css']
})
export class ClassManagementComponent implements OnInit {
  private classService = inject(ClassService);
  private gradeLevelService = inject(GradeLevelService);
  private schoolYearService = inject(SchoolYearService);
  private authService = inject(AuthService);

  classList: ClassDto[] = [];
  gradeLevelList: GradeLevelDto[] = [];
  schoolYearList: SchoolYearDto[] = [];
  eligibleTeacherList: TeacherDto[] = [];
  selectedClass: ClassDto | null = null;
  showCreateModal = false;
  showEditModal = false;
  showAssignTeacherModal = false;
  loading = false;
  errorMessage = '';
  successMessage = '';
  selectedTeacherId = '';
  isPrincipal = false;
  toasts: Toast[] = [];

  newClass: CreateClassDto = {
    classId: '',
    className: '',
    gradeLevelId: '',
    schoolYearId: '',
    classSize: 45
  };

  editClass: UpdateClassDto = {
    classId: '',
    className: '',
    gradeLevelId: '',
    schoolYearId: '',
    classSize: 45
  };

  ngOnInit(): void {
    this.isPrincipal = this.authService.hasRole(['Principal']);
    this.loadClasses();
    this.loadGradeLevels();
    this.loadSchoolYears();
  }

  loadClasses(): void {
    this.loading = true;
    this.classService.getAllClasses().subscribe({
      next: (data) => {
        this.classList = data;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách lớp học');
        this.loading = false;
      }
    });
  }

  loadGradeLevels(): void {
    this.gradeLevelService.getAllGradeLevels().subscribe({
      next: (data) => {
        this.gradeLevelList = data;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách khối');
      }
    });
  }

  loadSchoolYears(): void {
    this.schoolYearService.getAllSchoolYears().subscribe({
      next: (data) => {
        this.schoolYearList = data;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách năm học');
      }
    });
  }

  openCreateModal(): void {
    this.newClass = {
      classId: '',
      className: '',
      gradeLevelId: '',
      schoolYearId: '',
      classSize: 45
    };
    this.showCreateModal = true;
    this.clearMessages();
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  createClass(): void {
    if (!this.newClass.classId || !this.newClass.className || !this.newClass.gradeLevelId ||
        !this.newClass.schoolYearId || !this.newClass.classSize) {
      this.showError('Vui lòng điền đầy đủ thông tin');
      return;
    }

    if (this.newClass.classSize < 1 || this.newClass.classSize > 100) {
      this.showError('Sĩ số phải từ 1 đến 100');
      return;
    }

    this.loading = true;
    this.classService.createClass(this.newClass).subscribe({
      next: (data) => {
        this.showSuccess('Thêm lớp học thành công');
        this.loadClasses();
        this.closeCreateModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể thêm lớp học');
        this.loading = false;
      }
    });
  }

  openEditModal(classItem: ClassDto): void {
    this.selectedClass = classItem;
    this.editClass = {
      classId: classItem.classId,
      className: classItem.className,
      gradeLevelId: classItem.gradeLevelId,
      schoolYearId: classItem.schoolYearId,
      classSize: classItem.classSize
    };
    this.showEditModal = true;
    this.clearMessages();
  }

  closeEditModal(): void {
    this.showEditModal = false;
    this.selectedClass = null;
  }

  updateClass(): void {
    if (!this.selectedClass || !this.editClass.classId || !this.editClass.className ||
        !this.editClass.gradeLevelId || !this.editClass.schoolYearId || !this.editClass.classSize) {
      this.showError('Vui lòng điền đầy đủ thông tin');
      return;
    }

    if (this.editClass.classSize < 1 || this.editClass.classSize > 100) {
      this.showError('Sĩ số phải từ 1 đến 100');
      return;
    }

    this.loading = true;
    this.classService.updateClass(this.selectedClass.classId, this.editClass).subscribe({
      next: (data) => {
        this.showSuccess('Cập nhật lớp học thành công');
        this.loadClasses();
        this.closeEditModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể cập nhật lớp học');
        this.loading = false;
      }
    });
  }

  deleteClass(classItem: ClassDto): void {
    if (!confirm(`Bạn có chắc chắn muốn xóa lớp học "${classItem.className}"?`)) {
      return;
    }

    this.loading = true;
    this.classService.deleteClass(classItem.classId).subscribe({
      next: () => {
        this.showSuccess('Xóa lớp học thành công');
        this.loadClasses();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể xóa lớp học');
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

  openAssignTeacherModal(classItem: ClassDto): void {
    this.selectedClass = classItem;
    this.selectedTeacherId = classItem.teacherId || '';
    this.loading = true;
    this.classService.getEligibleHomeroomTeachers(classItem.classId).subscribe({
      next: (data) => {
        this.eligibleTeacherList = data;
        this.showAssignTeacherModal = true;
        this.loading = false;
        this.clearMessages();
        if (this.eligibleTeacherList.length === 0) {
          this.showError('Không có giáo viên nào đủ điều kiện. Giáo viên phải giảng dạy ít nhất một môn trong lớp này.');
        }
      },
      error: (error) => {
        this.showError('Không thể tải danh sách giáo viên');
        this.loading = false;
      }
    });
  }

  closeAssignTeacherModal(): void {
    this.showAssignTeacherModal = false;
    this.selectedClass = null;
    this.selectedTeacherId = '';
    this.eligibleTeacherList = [];
  }

  assignHomeroomTeacher(): void {
    if (!this.selectedClass || !this.selectedTeacherId) {
      this.showError('Vui lòng chọn giáo viên');
      return;
    }

    this.loading = true;
    this.classService.assignHomeroomTeacher(this.selectedClass.classId, this.selectedTeacherId).subscribe({
      next: (data) => {
        this.showSuccess('Phân công giáo viên chủ nhiệm thành công');
        this.loadClasses();
        this.closeAssignTeacherModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể phân công giáo viên chủ nhiệm');
        this.loading = false;
      }
    });
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }
}
