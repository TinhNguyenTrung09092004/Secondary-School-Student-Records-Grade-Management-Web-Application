import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClassAssignmentService } from '../../../services/classassignment.service';
import { ClassService } from '../../../services/class.service';
import { SchoolYearService } from '../../../services/schoolyear.service';
import { GradeLevelService } from '../../../services/gradelevel.service';
import {
  ClassInfoDto,
  StudentInClassDto,
  BulkAssignStudentsDto,
  RemoveStudentFromClassDto
} from '../../../models/classassignment.model';
import { ClassDto } from '../../../models/class.model';
import { SchoolYearDto } from '../../../models/schoolyear.model';
import { GradeLevelDto } from '../../../models/gradelevel.model';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-class-assignment',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './class-assignment.component.html',
  styleUrls: ['./class-assignment.component.css']
})
export class ClassAssignmentComponent implements OnInit {
  private classAssignmentService = inject(ClassAssignmentService);
  private classService = inject(ClassService);
  private schoolYearService = inject(SchoolYearService);
  private gradeLevelService = inject(GradeLevelService);

  schoolYearList: SchoolYearDto[] = [];
  gradeLevelList: GradeLevelDto[] = [];
  classList: ClassDto[] = [];

  selectedSchoolYearId = '';
  selectedGradeLevelId = '';
  selectedClassId = '';

  classInfo: ClassInfoDto | null = null;
  assignedStudents: StudentInClassDto[] = [];
  availableStudents: StudentInClassDto[] = [];

  selectedAssignedStudents: string[] = [];
  selectedAvailableStudents: string[] = [];

  loading = false;
  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.loadInitialData();
  }

  loadInitialData(): void {
    this.loading = true;
    forkJoin({
      schoolYears: this.schoolYearService.getAllSchoolYears(),
      gradeLevels: this.gradeLevelService.getAllGradeLevels()
    }).subscribe({
      next: (data) => {
        this.schoolYearList = data.schoolYears;
        this.gradeLevelList = data.gradeLevels;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải dữ liệu');
        this.loading = false;
      }
    });
  }

  onSchoolYearChange(): void {
    this.selectedClassId = '';
    this.classList = [];
    this.clearSelections();
    if (this.selectedSchoolYearId && this.selectedGradeLevelId) {
      this.loadClasses();
    }
  }

  onGradeLevelChange(): void {
    this.selectedClassId = '';
    this.classList = [];
    this.clearSelections();
    if (this.selectedSchoolYearId && this.selectedGradeLevelId) {
      this.loadClasses();
    }
  }

  loadClasses(): void {
    this.loading = true;
    this.classService.getClassesByYearAndGrade(this.selectedSchoolYearId, this.selectedGradeLevelId).subscribe({
      next: (classes) => {
        this.classList = classes;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách lớp');
        this.loading = false;
      }
    });
  }

  onClassChange(): void {
    this.clearSelections();
    if (this.selectedSchoolYearId && this.selectedGradeLevelId && this.selectedClassId) {
      this.loadClassData();
    }
  }

  loadClassData(): void {
    this.loading = true;
    forkJoin({
      classInfo: this.classAssignmentService.getClassInfo(this.selectedSchoolYearId, this.selectedGradeLevelId, this.selectedClassId),
      assignedStudents: this.classAssignmentService.getStudentsInClass(this.selectedSchoolYearId, this.selectedGradeLevelId, this.selectedClassId),
      availableStudents: this.classAssignmentService.getAvailableStudents(this.selectedSchoolYearId, this.selectedGradeLevelId)
    }).subscribe({
      next: (data) => {
        this.classInfo = data.classInfo;
        this.assignedStudents = data.assignedStudents;
        this.availableStudents = data.availableStudents;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải dữ liệu lớp');
        this.loading = false;
      }
    });
  }

  toggleAssignedStudent(studentId: string): void {
    const index = this.selectedAssignedStudents.indexOf(studentId);
    if (index > -1) {
      this.selectedAssignedStudents.splice(index, 1);
    } else {
      this.selectedAssignedStudents.push(studentId);
    }
  }

  toggleAvailableStudent(studentId: string): void {
    const index = this.selectedAvailableStudents.indexOf(studentId);
    if (index > -1) {
      this.selectedAvailableStudents.splice(index, 1);
    } else {
      this.selectedAvailableStudents.push(studentId);
    }
  }

  toggleAllAssigned(event: any): void {
    if (event.target.checked) {
      this.selectedAssignedStudents = this.assignedStudents.map(s => s.studentId);
    } else {
      this.selectedAssignedStudents = [];
    }
  }

  toggleAllAvailable(event: any): void {
    if (event.target.checked) {
      this.selectedAvailableStudents = this.availableStudents.map(s => s.studentId);
    } else {
      this.selectedAvailableStudents = [];
    }
  }

  assignSelectedStudents(): void {
    if (this.selectedAvailableStudents.length === 0) {
      this.showError('Vui lòng chọn học sinh để phân công');
      return;
    }

    const bulkAssignDto: BulkAssignStudentsDto = {
      schoolYearId: this.selectedSchoolYearId,
      gradeLevelId: this.selectedGradeLevelId,
      classId: this.selectedClassId,
      studentIds: this.selectedAvailableStudents
    };

    this.loading = true;
    this.classAssignmentService.bulkAssignStudents(bulkAssignDto).subscribe({
      next: () => {
        this.showSuccess('Phân công học sinh thành công');
        this.loadClassData();
        this.selectedAvailableStudents = [];
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể phân công học sinh');
        this.loading = false;
      }
    });
  }

  removeSelectedStudents(): void {
    if (this.selectedAssignedStudents.length === 0) {
      this.showError('Vui lòng chọn học sinh để xóa khỏi lớp');
      return;
    }

    if (!confirm(`Bạn có chắc chắn muốn xóa ${this.selectedAssignedStudents.length} học sinh khỏi lớp?`)) {
      return;
    }

    this.loading = true;
    const removePromises = this.selectedAssignedStudents.map(studentId => {
      const removeDto: RemoveStudentFromClassDto = {
        schoolYearId: this.selectedSchoolYearId,
        gradeLevelId: this.selectedGradeLevelId,
        classId: this.selectedClassId,
        studentId: studentId
      };
      return this.classAssignmentService.removeStudentFromClass(removeDto).toPromise();
    });

    Promise.all(removePromises).then(
      () => {
        this.showSuccess('Xóa học sinh khỏi lớp thành công');
        this.loadClassData();
        this.selectedAssignedStudents = [];
      },
      (error) => {
        this.showError('Không thể xóa một số học sinh');
        this.loading = false;
      }
    );
  }

  clearSelections(): void {
    this.selectedAssignedStudents = [];
    this.selectedAvailableStudents = [];
    this.classInfo = null;
    this.assignedStudents = [];
    this.availableStudents = [];
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
