import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TeacherService } from '../../../services/teacher.service';
import { SubjectService } from '../../../services/subject.service';
import { DepartmentService } from '../../../services/department.service';
import { TeacherDto, CreateTeacherProfileDto, UpdateTeacherProfileDto } from '../../../models/teacher.model';
import { SubjectDto } from '../../../models/subject.model';
import { DepartmentDto } from '../../../models/department.model';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-teacher-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './teacher-profile.component.html',
  styleUrls: ['./teacher-profile.component.css']
})
export class TeacherProfileComponent implements OnInit {
  private teacherService = inject(TeacherService);
  private subjectService = inject(SubjectService);
  private departmentService = inject(DepartmentService);

  profile: TeacherDto | null = null;
  subjectList: SubjectDto[] = [];
  departmentList: DepartmentDto[] = [];

  hasProfile = false;
  showCreateModal = false;
  showEditModal = false;
  loading = false;
  errorMessage = '';
  successMessage = '';

  newProfile: CreateTeacherProfileDto = {
    teacherId: '',
    teacherName: '',
    address: '',
    phoneNumber: '',
    subjectId: '',
    departmentId: ''
  };

  editProfile: UpdateTeacherProfileDto = {
    teacherName: '',
    address: '',
    phoneNumber: '',
    subjectId: '',
    departmentId: ''
  };

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading = true;
    forkJoin({
      subjects: this.subjectService.getAllSubjects(),
      departments: this.departmentService.getAllDepartments()
    }).subscribe({
      next: (data) => {
        this.subjectList = data.subjects;
        this.departmentList = data.departments;
        this.loadProfile();
      },
      error: (error) => {
        this.showError('Không thể tải dữ liệu');
        this.loading = false;
      }
    });
  }

  loadProfile(): void {
    this.teacherService.getProfile().subscribe({
      next: (profile) => {
        this.profile = profile;
        this.hasProfile = true;
        this.loading = false;
      },
      error: (error) => {
        if (error.status === 404) {
          this.hasProfile = false;
        } else {
          this.showError('Không thể tải hồ sơ');
        }
        this.loading = false;
      }
    });
  }

  openCreateModal(): void {
    this.newProfile = {
      teacherId: '',
      teacherName: '',
      address: '',
      phoneNumber: '',
      subjectId: '',
      departmentId: ''
    };
    this.showCreateModal = true;
    this.clearMessages();
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  createProfile(): void {
    if (!this.validateProfileData(this.newProfile, true)) {
      return;
    }

    this.loading = true;
    this.teacherService.createProfile(this.newProfile).subscribe({
      next: (profile) => {
        this.showSuccess('Tạo hồ sơ thành công');
        this.profile = profile;
        this.hasProfile = true;
        this.closeCreateModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể tạo hồ sơ');
        this.loading = false;
      }
    });
  }

  openEditModal(): void {
    if (!this.profile) return;

    this.editProfile = {
      teacherName: this.profile.teacherName,
      address: this.profile.address,
      phoneNumber: this.profile.phoneNumber,
      subjectId: this.profile.subjectId,
      departmentId: this.profile.departmentId || ''
    };
    this.showEditModal = true;
    this.clearMessages();
  }

  closeEditModal(): void {
    this.showEditModal = false;
  }

  updateProfile(): void {
    if (!this.validateProfileData(this.editProfile, false)) {
      return;
    }

    this.loading = true;
    this.teacherService.updateProfile(this.editProfile).subscribe({
      next: (profile) => {
        this.showSuccess('Cập nhật hồ sơ thành công');
        this.profile = profile;
        this.closeEditModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể cập nhật hồ sơ');
        this.loading = false;
      }
    });
  }

  deleteProfile(): void {
    if (!confirm('Bạn có chắc chắn muốn xóa hồ sơ của mình?')) {
      return;
    }

    this.loading = true;
    this.teacherService.deleteProfile().subscribe({
      next: () => {
        this.showSuccess('Xóa hồ sơ thành công');
        this.profile = null;
        this.hasProfile = false;
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể xóa hồ sơ');
        this.loading = false;
      }
    });
  }

  validateProfileData(profile: CreateTeacherProfileDto | UpdateTeacherProfileDto, isCreate: boolean): boolean {
    if (isCreate && 'teacherId' in profile) {
      if (!profile.teacherId) {
        this.showError('Mã giáo viên là bắt buộc');
        return false;
      }
    }

    if (!profile.teacherName || !profile.address || !profile.phoneNumber || !profile.subjectId) {
      this.showError('Vui lòng điền đầy đủ thông tin bắt buộc');
      return false;
    }

    return true;
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

  getSubjectName(subjectId: string): string {
    const subject = this.subjectList.find(s => s.subjectId === subjectId);
    return subject ? subject.subjectName : subjectId;
  }

  getDepartmentName(departmentId?: string): string {
    if (!departmentId) return 'Chưa có';
    const department = this.departmentList.find(d => d.departmentId === departmentId);
    return department ? department.departmentName : departmentId;
  }
}
