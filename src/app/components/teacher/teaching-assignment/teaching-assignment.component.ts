import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TeachingAssignmentService } from '../../../services/teachingassignment.service';
import { ClassService } from '../../../services/class.service';
import { SchoolYearService } from '../../../services/schoolyear.service';
import { SubjectService } from '../../../services/subject.service';
import {
  TeachingAssignmentDto,
  CreateTeachingAssignmentDto,
  UpdateTeachingAssignmentDto,
  DepartmentTeacherDto
} from '../../../models/teachingassignment.model';
import { ClassDto } from '../../../models/class.model';
import { SchoolYearDto } from '../../../models/schoolyear.model';
import { SubjectDto } from '../../../models/subject.model';

@Component({
  selector: 'app-teaching-assignment',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './teaching-assignment.component.html',
  styleUrl: './teaching-assignment.component.css'
})
export class TeachingAssignmentComponent implements OnInit {
  private teachingAssignmentService = inject(TeachingAssignmentService);
  private classService = inject(ClassService);
  private schoolYearService = inject(SchoolYearService);
  private subjectService = inject(SubjectService);

  assignmentList: TeachingAssignmentDto[] = [];
  teacherList: DepartmentTeacherDto[] = [];
  classList: ClassDto[] = [];
  schoolYearList: SchoolYearDto[] = [];
  subjectList: SubjectDto[] = [];

  showCreateModal = false;
  showEditModal = false;
  selectedAssignment: TeachingAssignmentDto | null = null;
  loading = false;
  errorMessage = '';
  successMessage = '';

  newAssignment: CreateTeachingAssignmentDto = {
    schoolYearId: '',
    classId: '',
    subjectId: '',
    teacherId: ''
  };

  editAssignment: UpdateTeachingAssignmentDto = {
    schoolYearId: '',
    classId: '',
    subjectId: '',
    teacherId: ''
  };

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loadAssignments();
    this.loadTeachers();
    this.loadClasses();
    this.loadSchoolYears();
    this.loadSubjects();
  }

  loadAssignments(): void {
    this.loading = true;
    this.teachingAssignmentService.getAllTeachingAssignments().subscribe({
      next: (data) => {
        this.assignmentList = data;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải dữ liệu phân công');
        this.loading = false;
      }
    });
  }

  loadTeachers(): void {
    this.teachingAssignmentService.getDepartmentTeachers().subscribe({
      next: (data) => {
        this.teacherList = data;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách giáo viên');
      }
    });
  }

  loadClasses(): void {
    this.classService.getAllClasses().subscribe({
      next: (data) => {
        this.classList = data;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách lớp học');
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

  loadSubjects(): void {
    this.subjectService.getAllSubjects().subscribe({
      next: (data) => {
        this.subjectList = data;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách môn học');
      }
    });
  }

  openCreateModal(): void {
    this.newAssignment = {
      schoolYearId: '',
      classId: '',
      subjectId: '',
      teacherId: ''
    };
    this.showCreateModal = true;
    this.clearMessages();
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  createAssignment(): void {
    if (!this.newAssignment.schoolYearId || !this.newAssignment.classId ||
        !this.newAssignment.subjectId || !this.newAssignment.teacherId) {
      this.showError('Vui lòng điền đầy đủ thông tin');
      return;
    }

    this.loading = true;
    this.teachingAssignmentService.createTeachingAssignment(this.newAssignment).subscribe({
      next: (data) => {
        this.showSuccess('Phân công giảng dạy thành công');
        this.loadAssignments();
        this.closeCreateModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể phân công giảng dạy');
        this.loading = false;
      }
    });
  }

  openEditModal(assignment: TeachingAssignmentDto): void {
    this.selectedAssignment = assignment;
    this.editAssignment = {
      schoolYearId: assignment.schoolYearId,
      classId: assignment.classId,
      subjectId: assignment.subjectId,
      teacherId: assignment.teacherId
    };
    this.showEditModal = true;
    this.clearMessages();
  }

  closeEditModal(): void {
    this.showEditModal = false;
    this.selectedAssignment = null;
  }

  updateAssignment(): void {
    if (!this.selectedAssignment || !this.editAssignment.schoolYearId ||
        !this.editAssignment.classId || !this.editAssignment.subjectId ||
        !this.editAssignment.teacherId) {
      this.showError('Vui lòng điền đầy đủ thông tin');
      return;
    }

    this.loading = true;
    this.teachingAssignmentService.updateTeachingAssignment(this.selectedAssignment.rowNumber, this.editAssignment).subscribe({
      next: (data) => {
        this.showSuccess('Cập nhật phân công giảng dạy thành công');
        this.loadAssignments();
        this.closeEditModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể cập nhật phân công giảng dạy');
        this.loading = false;
      }
    });
  }

  deleteAssignment(assignment: TeachingAssignmentDto): void {
    if (!confirm(`Bạn có chắc chắn muốn xóa phân công giảng dạy của giáo viên "${assignment.teacherName}" môn "${assignment.subjectName}" lớp "${assignment.className}"?`)) {
      return;
    }

    this.loading = true;
    this.teachingAssignmentService.deleteTeachingAssignment(assignment.rowNumber).subscribe({
      next: () => {
        this.showSuccess('Xóa phân công giảng dạy thành công');
        this.loadAssignments();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể xóa phân công giảng dạy');
        this.loading = false;
      }
    });
  }

  getMyAssignments(): TeachingAssignmentDto[] {
    const myTeacherIds = this.teacherList.map(t => t.teacherId);
    return this.assignmentList.filter(a => myTeacherIds.includes(a.teacherId));
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
