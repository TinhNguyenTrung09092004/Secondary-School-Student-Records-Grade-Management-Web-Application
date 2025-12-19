import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  PrincipalDashboardService,
  GradeLevelResultsDto,
  StudentResultDto
} from '../../../services/principal-dashboard.service';

@Component({
  selector: 'app-principal-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './principal-dashboard.component.html',
  styleUrl: './principal-dashboard.component.css'
})
export class PrincipalDashboardComponent implements OnInit {
  private dashboardService = inject(PrincipalDashboardService);

  gradeLevelResults: GradeLevelResultsDto[] = [];
  selectedSchoolYearId: string = '';
  selectedView: 'semester1' | 'semester2' | 'year' = 'year';
  expandedStudents: Set<string> = new Set();
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  // Available school years (you might want to load this from a service)
  schoolYears = [
    { id: '2024-2025', name: '2024-2025' },
    { id: '2023-2024', name: '2023-2024' }
  ];

  ngOnInit(): void {
    if (this.schoolYears.length > 0) {
      this.selectedSchoolYearId = this.schoolYears[0].id;
      this.loadResults();
    }
  }

  loadResults(): void {
    if (!this.selectedSchoolYearId) return;

    this.isLoading = true;
    this.dashboardService.getGradeLevelResults(this.selectedSchoolYearId).subscribe({
      next: (data) => {
        this.gradeLevelResults = data;
        this.isLoading = false;
      },
      error: (error) => {
        this.showError('Không thể tải dữ liệu');
        this.isLoading = false;
      }
    });
  }

  recalculateAll(): void {
    if (!this.selectedSchoolYearId) return;

    if (!confirm('Bạn có chắc chắn muốn tính toán lại tất cả học lực?')) return;

    this.isLoading = true;
    this.dashboardService.recalculateAllRankings(this.selectedSchoolYearId).subscribe({
      next: (response) => {
        this.showSuccess(response.message || 'Đã tính toán lại thành công');
        this.loadResults();
      },
      error: (error) => {
        this.showError('Không thể tính toán lại học lực');
        this.isLoading = false;
      }
    });
  }

  toggleStudentDetails(studentId: string): void {
    if (this.expandedStudents.has(studentId)) {
      this.expandedStudents.delete(studentId);
    } else {
      this.expandedStudents.add(studentId);
    }
  }

  isStudentExpanded(studentId: string): boolean {
    return this.expandedStudents.has(studentId);
  }

  getDisplayedAverage(student: StudentResultDto): number | undefined {
    switch (this.selectedView) {
      case 'semester1':
        return student.semester1Average;
      case 'semester2':
        return student.semester2Average;
      case 'year':
        return student.yearAverage;
    }
  }

  getDisplayedAcademicPerformance(student: StudentResultDto): string | undefined {
    switch (this.selectedView) {
      case 'semester1':
        return student.semester1AcademicPerformance;
      case 'semester2':
        return student.semester2AcademicPerformance;
      case 'year':
        return student.yearAcademicPerformance;
    }
  }

  getDisplayedConduct(student: StudentResultDto): string | undefined {
    switch (this.selectedView) {
      case 'semester1':
        return student.semester1Conduct;
      case 'semester2':
        return student.semester2Conduct;
      case 'year':
        return student.yearConduct;
    }
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
