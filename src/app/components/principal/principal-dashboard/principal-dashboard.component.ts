import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  PrincipalDashboardService,
  GradeLevelResultsDto,
  StudentResultDto,
  SubjectDetailedGradesDto
} from '../../../services/principal-dashboard.service';
import { SchoolYearService } from '../../../services/schoolyear.service';
import { SchoolYearDto } from '../../../models/schoolyear.model';

interface DashboardStats {
  totalStudents: number;
  totalClasses: number;
  averageScore: number;
  excellentStudents: number;
  goodStudents: number;
  averageStudents: number;
  belowAverageStudents: number;
}

interface PerformanceDistribution {
  excellent: number;
  good: number;
  average: number;
  belowAverage: number;
}

interface ClassStats {
  className: string;
  studentCount: number;
  averageScore: number;
}

@Component({
  selector: 'app-principal-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './principal-dashboard.component.html',
  styleUrl: './principal-dashboard.component.css'
})
export class PrincipalDashboardComponent implements OnInit {
  private dashboardService = inject(PrincipalDashboardService);
  private schoolYearService = inject(SchoolYearService);

  gradeLevelResults: GradeLevelResultsDto[] = [];
  selectedSchoolYearId: string = '';
  selectedView: 'semester1' | 'semester2' | 'year' = 'year';
  expandedStudents: Set<string> = new Set();
  expandedSubjects: Map<string, SubjectDetailedGradesDto> = new Map();
  loadingSubjects: Set<string> = new Set();
  isLoading = false;
  errorMessage = '';
  schoolYears: SchoolYearDto[] = [];

  // View mode: 'overview' for dashboard, 'details' for student grades
  viewMode: 'overview' | 'details' = 'overview';

  // Dashboard statistics
  stats: DashboardStats = {
    totalStudents: 0,
    totalClasses: 0,
    averageScore: 0,
    excellentStudents: 0,
    goodStudents: 0,
    averageStudents: 0,
    belowAverageStudents: 0
  };

  performanceDistribution: PerformanceDistribution = {
    excellent: 0,
    good: 0,
    average: 0,
    belowAverage: 0
  };

  classStats: ClassStats[] = [];
  conductDistribution: { [key: string]: number } = {};
  semesterComparison: { semester1Avg: number; semester2Avg: number; yearAvg: number } = {
    semester1Avg: 0,
    semester2Avg: 0,
    yearAvg: 0
  };

  ngOnInit(): void {
    this.loadSchoolYears();
  }

  loadSchoolYears(): void {
    this.schoolYearService.getAllSchoolYears().subscribe({
      next: (data) => {
        this.schoolYears = data;
        if (this.schoolYears.length > 0) {
          this.selectedSchoolYearId = this.schoolYears[0].schoolYearId;
          this.loadResults();
        }
      },
      error: (error) => {
        this.showError('Không thể tải danh sách năm học');
      }
    });
  }

  loadResults(): void {
    if (!this.selectedSchoolYearId) return;

    this.isLoading = true;
    this.dashboardService.getGradeLevelResults(this.selectedSchoolYearId).subscribe({
      next: (data) => {
        this.gradeLevelResults = data;
        this.calculateStatistics();
        this.isLoading = false;
      },
      error: (error) => {
        this.showError('Không thể tải dữ liệu');
        this.isLoading = false;
      }
    });
  }

  calculateStatistics(): void {
    let totalStudents = 0;
    let totalScore = 0;
    let totalClasses = 0;
    let excellent = 0;
    let good = 0;
    let average = 0;
    let belowAverage = 0;

    const conductCount: { [key: string]: number } = {};
    const classStatsTemp: ClassStats[] = [];

    let semester1Total = 0;
    let semester2Total = 0;
    let yearTotal = 0;
    let semester1Count = 0;
    let semester2Count = 0;
    let yearCount = 0;

    this.gradeLevelResults.forEach(gradeLevel => {
      gradeLevel.classes.forEach(classItem => {
        totalClasses++;
        let classTotal = 0;
        let classCount = 0;

        classItem.students.forEach(student => {
          totalStudents++;

          // Year average
          if (student.yearAverage !== undefined && student.yearAverage !== null) {
            totalScore += student.yearAverage;
            classTotal += student.yearAverage;
            classCount++;
          }

          // Semester averages
          if (student.semester1Average) {
            semester1Total += student.semester1Average;
            semester1Count++;
          }
          if (student.semester2Average) {
            semester2Total += student.semester2Average;
            semester2Count++;
          }
          if (student.yearAverage) {
            yearTotal += student.yearAverage;
            yearCount++;
          }

          // Academic performance distribution
          const performance = student.yearAcademicPerformance || student.semester2AcademicPerformance || student.semester1AcademicPerformance;
          if (performance) {
            if (performance === 'Xuất sắc' || performance === 'Tốt') {
              excellent++;
            } else if (performance === 'Khá') {
              good++;
            } else if (performance === 'Đạt') {
              average++;
            } else {
              belowAverage++;
            }
          }

          // Conduct distribution
          const conduct = student.yearConduct || student.semester2Conduct || student.semester1Conduct;
          if (conduct) {
            conductCount[conduct] = (conductCount[conduct] || 0) + 1;
          }
        });

        // Class statistics
        if (classCount > 0) {
          classStatsTemp.push({
            className: classItem.className,
            studentCount: classItem.students.length,
            averageScore: classTotal / classCount
          });
        }
      });
    });

    // Update stats
    this.stats = {
      totalStudents,
      totalClasses,
      averageScore: totalStudents > 0 ? totalScore / totalStudents : 0,
      excellentStudents: excellent,
      goodStudents: good,
      averageStudents: average,
      belowAverageStudents: belowAverage
    };

    this.performanceDistribution = {
      excellent,
      good,
      average,
      belowAverage
    };

    this.classStats = classStatsTemp.slice(0, 10); // Top 10 classes
    this.conductDistribution = conductCount;

    this.semesterComparison = {
      semester1Avg: semester1Count > 0 ? semester1Total / semester1Count : 0,
      semester2Avg: semester2Count > 0 ? semester2Total / semester2Count : 0,
      yearAvg: yearCount > 0 ? yearTotal / yearCount : 0
    };
  }

  // Original detailed view methods
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

  toggleSubjectDetails(studentId: string, classId: string, subjectId: string): void {
    const key = `${studentId}-${subjectId}`;

    if (this.expandedSubjects.has(key)) {
      this.expandedSubjects.delete(key);
    } else {
      this.loadSubjectDetails(studentId, classId, subjectId);
    }
  }

  isSubjectExpanded(studentId: string, subjectId: string): boolean {
    const key = `${studentId}-${subjectId}`;
    return this.expandedSubjects.has(key);
  }

  isSubjectLoading(studentId: string, subjectId: string): boolean {
    const key = `${studentId}-${subjectId}`;
    return this.loadingSubjects.has(key);
  }

  getSubjectDetails(studentId: string, subjectId: string): SubjectDetailedGradesDto | undefined {
    const key = `${studentId}-${subjectId}`;
    return this.expandedSubjects.get(key);
  }

  loadSubjectDetails(studentId: string, classId: string, subjectId: string): void {
    const key = `${studentId}-${subjectId}`;
    this.loadingSubjects.add(key);

    this.dashboardService.getSubjectDetailedGrades(
      studentId,
      classId,
      this.selectedSchoolYearId,
      subjectId
    ).subscribe({
      next: (data) => {
        this.expandedSubjects.set(key, data);
        this.loadingSubjects.delete(key);
      },
      error: (error) => {
        this.showError('Không thể tải chi tiết điểm môn học');
        this.loadingSubjects.delete(key);
      }
    });
  }

  // Dashboard helper methods
  getPerformancePercentage(type: 'excellent' | 'good' | 'average' | 'belowAverage'): number {
    const total = this.performanceDistribution.excellent +
                  this.performanceDistribution.good +
                  this.performanceDistribution.average +
                  this.performanceDistribution.belowAverage;

    if (total === 0) return 0;
    return (this.performanceDistribution[type] / total) * 100;
  }

  getConductPercentage(conduct: string): number {
    const total = Object.values(this.conductDistribution).reduce((sum, val) => sum + val, 0);
    if (total === 0) return 0;
    return ((this.conductDistribution[conduct] || 0) / total) * 100;
  }

  getMaxClassCount(): number {
    return Math.max(...this.classStats.map(c => c.studentCount), 1);
  }

  getMaxAvgScore(): number {
    const maxScore = Math.max(...this.classStats.map(c => c.averageScore), 1);
    return Math.ceil(maxScore);
  }

  showError(message: string): void {
    this.errorMessage = message;
    setTimeout(() => this.errorMessage = '', 5000);
  }

  switchView(mode: 'overview' | 'details'): void {
    this.viewMode = mode;
  }
}
