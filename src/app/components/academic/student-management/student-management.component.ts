import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StudentService } from '../../../services/student.service';
import { EthnicityService } from '../../../services/ethnicity.service';
import { ReligionService } from '../../../services/religion.service';
import { OccupationService } from '../../../services/occupation.service';
import { StudentDto, CreateStudentDto, UpdateStudentDto } from '../../../models/hocsinh.model';
import { EthnicityDto } from '../../../models/ethnicity.model';
import { ReligionDto } from '../../../models/religion.model';
import { OccupationDto } from '../../../models/occupation.model';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-student-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './student-management.component.html',
  styleUrls: ['./student-management.component.css']
})
export class StudentManagementComponent implements OnInit {
  private studentService = inject(StudentService);
  private ethnicityService = inject(EthnicityService);
  private religionService = inject(ReligionService);
  private occupationService = inject(OccupationService);

  studentList: StudentDto[] = [];
  ethnicityList: EthnicityDto[] = [];
  religionList: ReligionDto[] = [];
  occupationList: OccupationDto[] = [];

  selectedStudent: StudentDto | null = null;
  showCreateModal = false;
  showEditModal = false;
  loading = false;
  errorMessage = '';
  successMessage = '';

  newStudent: CreateStudentDto = {
    studentId: '',
    fullName: '',
    gender: true,
    dateOfBirth: '',
    address: '',
    email: '',
    ethnicityId: '',
    religionId: '',
    fatherName: '',
    fatherOccupationId: '',
    motherName: '',
    motherOccupationId: ''
  };

  editStudent: UpdateStudentDto = {
    fullName: '',
    gender: true,
    dateOfBirth: '',
    address: '',
    email: '',
    ethnicityId: '',
    religionId: '',
    fatherName: '',
    fatherOccupationId: '',
    motherName: '',
    motherOccupationId: ''
  };

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading = true;
    forkJoin({
      students: this.studentService.getAllStudents(),
      ethnicities: this.ethnicityService.getAllEthnicity(),
      religions: this.religionService.getAllReligion(),
      occupations: this.occupationService.getAllOccupation()
    }).subscribe({
      next: (data) => {
        this.studentList = data.students;
        this.ethnicityList = data.ethnicities;
        this.religionList = data.religions;
        this.occupationList = data.occupations;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Không thể tải dữ liệu');
        this.loading = false;
      }
    });
  }

  openCreateModal(): void {
    this.newStudent = {
      studentId: '',
      fullName: '',
      gender: true,
      dateOfBirth: '',
      address: '',
      email: '',
      ethnicityId: '',
      religionId: '',
      fatherName: '',
      fatherOccupationId: '',
      motherName: '',
      motherOccupationId: ''
    };
    this.showCreateModal = true;
    this.clearMessages();
  }

  closeCreateModal(): void {
    this.showCreateModal = false;
  }

  createStudent(): void {
    if (!this.validateStudentData(this.newStudent)) {
      return;
    }

    this.loading = true;
    // Transform date to ISO format for API
    const studentData = {
      ...this.newStudent,
      dateOfBirth: new Date(this.newStudent.dateOfBirth).toISOString()
    };

    this.studentService.createStudent(studentData).subscribe({
      next: (data) => {
        this.showSuccess('Thêm học sinh thành công');
        this.loadData();
        this.closeCreateModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể thêm học sinh');
        this.loading = false;
      }
    });
  }

  openEditModal(student: StudentDto): void {
    this.selectedStudent = student;
    this.editStudent = {
      fullName: student.fullName,
      gender: student.gender,
      dateOfBirth: student.dateOfBirth.substring(0, 10),
      address: student.address,
      email: student.email,
      ethnicityId: student.ethnicityId,
      religionId: student.religionId,
      fatherName: student.fatherName,
      fatherOccupationId: student.fatherOccupationId,
      motherName: student.motherName,
      motherOccupationId: student.motherOccupationId
    };
    this.showEditModal = true;
    this.clearMessages();
  }

  closeEditModal(): void {
    this.showEditModal = false;
    this.selectedStudent = null;
  }

  updateStudent(): void {
    if (!this.selectedStudent || !this.validateStudentData(this.editStudent)) {
      return;
    }

    this.loading = true;
    // Transform date to ISO format for API
    const studentData = {
      ...this.editStudent,
      dateOfBirth: new Date(this.editStudent.dateOfBirth).toISOString()
    };

    this.studentService.updateStudent(this.selectedStudent.studentId, studentData).subscribe({
      next: (data) => {
        this.showSuccess('Cập nhật học sinh thành công');
        this.loadData();
        this.closeEditModal();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể cập nhật học sinh');
        this.loading = false;
      }
    });
  }

  deleteStudent(student: StudentDto): void {
    if (!confirm(`Bạn có chắc chắn muốn xóa học sinh "${student.fullName}"?`)) {
      return;
    }

    this.loading = true;
    this.studentService.deleteStudent(student.studentId).subscribe({
      next: () => {
        this.showSuccess('Xóa học sinh thành công');
        this.loadData();
        this.loading = false;
      },
      error: (error) => {
        this.showError(error.error?.message || 'Không thể xóa học sinh');
        this.loading = false;
      }
    });
  }

  validateStudentData(student: CreateStudentDto | UpdateStudentDto): boolean {
    // For CreateStudentDto, check studentId as well
    if ('studentId' in student) {
      if (!student.studentId || !student.fullName || !student.dateOfBirth || !student.address ||
          !student.email || !student.ethnicityId || !student.religionId ||
          !student.fatherName || !student.fatherOccupationId ||
          !student.motherName || !student.motherOccupationId) {
        this.showError('Vui lòng điền đầy đủ thông tin');
        return false;
      }
    } else {
      // For UpdateStudentDto
      if (!student.fullName || !student.dateOfBirth || !student.address ||
          !student.email || !student.ethnicityId || !student.religionId ||
          !student.fatherName || !student.fatherOccupationId ||
          !student.motherName || !student.motherOccupationId) {
        this.showError('Vui lòng điền đầy đủ thông tin');
        return false;
      }
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

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN');
  }
}
