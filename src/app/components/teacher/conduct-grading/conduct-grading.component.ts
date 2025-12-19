import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ConductGradingService } from '../../../services/conduct-grading.service';
import { ConductService } from '../../../services/conduct.service';
import { StudentConductDto, UpdateStudentConductDto, ClassForConductGradingDto } from '../../../models/conduct-grading.model';
import { ConductDto } from '../../../models/conduct.model';

@Component({
  selector: 'app-conduct-grading',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './conduct-grading.component.html',
  styleUrls: ['./conduct-grading.component.css']
})
export class ConductGradingComponent implements OnInit {
  private conductGradingService = inject(ConductGradingService);
  private conductService = inject(ConductService);

  myClasses: ClassForConductGradingDto[] = [];
  selectedClass: ClassForConductGradingDto | null = null;
  students: StudentConductDto[] = [];
  conductList: ConductDto[] = [];

  loading = false;
  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.loadMyClasses();
    this.loadConductList();
  }

  loadMyClasses(): void {
    this.loading = true;
    this.conductGradingService.getMyClasses().subscribe({
      next: (data) => {
        this.myClasses = data;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách lớp của bạn');
        this.loading = false;
      }
    });
  }

  loadConductList(): void {
    this.conductService.getAllConduct().subscribe({
      next: (data) => {
        this.conductList = data;
      },
      error: (error) => {
        this.showError('Không thể tải danh sách hạnh kiểm');
      }
    });
  }

  selectClass(classItem: ClassForConductGradingDto): void {
    this.selectedClass = classItem;
    this.loadStudents();
  }

  loadStudents(): void {
    if (!this.selectedClass) return;

    this.loading = true;
    this.conductGradingService.getStudentsForGrading(
      this.selectedClass.classId,
      this.selectedClass.schoolYearId
    ).subscribe({
      next: (data) => {
        this.students = data;
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể tải danh sách học sinh');
        this.loading = false;
      }
    });
  }

  updateConduct(student: StudentConductDto, semesterId: string, conductId: string): void {
    if (!conductId) {
      this.showError('Vui lòng chọn hạnh kiểm');
      return;
    }

    const semesterName = semesterId === 'HK1' ? 'Học kỳ I' : 'Học kỳ II';
    const updateDto: UpdateStudentConductDto = {
      studentId: student.studentId,
      classId: student.classId,
      schoolYearId: student.schoolYearId,
      semesterId: semesterId,
      conductId: conductId
    };

    this.loading = true;
    this.conductGradingService.updateStudentConduct(updateDto).subscribe({
      next: () => {
        this.showSuccess(`Cập nhật hạnh kiểm ${semesterName} cho ${student.studentName} thành công`);
        this.loadStudents();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể cập nhật hạnh kiểm');
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
