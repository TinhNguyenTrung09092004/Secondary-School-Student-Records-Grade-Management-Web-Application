import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';
import { TokenValidatorComponent } from './components/token-validator/token-validator.component';
import { FaceEnrollmentComponent } from './components/face-enrollment/face-enrollment.component';
import { FaceLoginComponent } from './components/face-login/face-login.component';
import { AdminLayoutComponent } from './components/layouts/admin-layout/admin-layout.component';
import { AcademicLayoutComponent } from './components/layouts/academic-layout/academic-layout.component';
import { TeacherLayoutComponent } from './components/layouts/teacher-layout/teacher-layout.component';
import { UserManagementComponent } from './components/admin/user-management/user-management.component';
import { BackupComponent } from './components/admin/backup/backup.component';
import { LogoSettingsComponent } from './components/admin/logo-settings/logo-settings.component';
import { EthnicityManagementComponent } from './components/academic/ethnicity-management/ethnicity-management.component';
import { ReligionManagementComponent } from './components/academic/religion-management/religion-management.component';
import { OccupationManagementComponent } from './components/academic/occupation-management/occupation-management.component';
import { StudentManagementComponent } from './components/academic/student-management/student-management.component';
import { SubjectManagementComponent } from './components/academic/subject-management/subject-management.component';
import { DepartmentManagementComponent } from './components/academic/department-management/department-management.component';
import { GradeLevelManagementComponent } from './components/academic/grade-level-management/grade-level-management.component';
import { SchoolYearManagementComponent } from './components/academic/school-year-management/school-year-management.component';
import { ClassAssignmentComponent } from './components/academic/class-assignment/class-assignment.component';
import { ClassManagementComponent } from './components/academic/class-management/class-management.component';
import { SemesterManagementComponent } from './components/academic/semester-management/semester-management.component';
import { GradeTypeManagementComponent } from './components/academic/grade-type-management/grade-type-management.component';
import { ConductManagementComponent } from './components/academic/conduct-management/conduct-management.component';
import { AcademicPerformanceManagementComponent } from './components/academic/academic-performance-management/academic-performance-management.component';
import { ResultManagementComponent } from './components/academic/result-management/result-management.component';
import { TeacherProfileComponent } from './components/teacher/teacher-profile/teacher-profile.component';
import { TeachingAssignmentComponent } from './components/teacher/teaching-assignment/teaching-assignment.component';
import { GradeEntryComponent } from './components/teacher/grade-entry/grade-entry.component';
import { GradeViewComponent } from './components/teacher/grade-view/grade-view.component';
import { ConductGradingComponent } from './components/teacher/conduct-grading/conduct-grading.component';
import { PrincipalDashboardComponent } from './components/principal/principal-dashboard/principal-dashboard.component';
import { authGuard } from './guards/auth.guard';
import { roleGuard } from './guards/role.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'face-login', component: FaceLoginComponent },
  { path: 'face-enrollment', component: FaceEnrollmentComponent, canActivate: [authGuard] },
  { path: 'forgot-password', component: ForgotPasswordComponent },
  { path: 'validate-token', component: TokenValidatorComponent },
  { path: 'reset-password', component: ResetPasswordComponent },
  { path: 'setup-account', component: ResetPasswordComponent },
  {
    path: 'admin',
    component: AdminLayoutComponent,
    canActivate: [authGuard, roleGuard(['Admin'])],
    children: [
      { path: 'users', component: UserManagementComponent },
      { path: 'backup', component: BackupComponent },
      { path: 'logo-settings', component: LogoSettingsComponent }
    ]
  },
  {
    path: 'academic',
    component: AcademicLayoutComponent,
    canActivate: [authGuard, roleGuard(['AcademicAffairs', 'Principal', 'DepartmentHead'])],
    children: [
      { path: 'ethnicity', component: EthnicityManagementComponent },
      { path: 'religion', component: ReligionManagementComponent },
      { path: 'occupation', component: OccupationManagementComponent },
      { path: 'students', component: StudentManagementComponent },
      { path: 'classes', component: ClassManagementComponent },
      { path: 'class-assignment', component: ClassAssignmentComponent },
      { path: 'grade-levels', component: GradeLevelManagementComponent },
      { path: 'school-years', component: SchoolYearManagementComponent },
      { path: 'subjects', component: SubjectManagementComponent },
      { path: 'departments', component: DepartmentManagementComponent },
      { path: 'semesters', component: SemesterManagementComponent },
      { path: 'grade-types', component: GradeTypeManagementComponent },
      { path: 'conduct', component: ConductManagementComponent },
      { path: 'academic-performance', component: AcademicPerformanceManagementComponent },
      { path: 'result', component: ResultManagementComponent },
      { path: 'principal-dashboard', component: PrincipalDashboardComponent }
    ]
  },
  {
    path: 'teacher',
    component: TeacherLayoutComponent,
    canActivate: [authGuard, roleGuard(['SubjectTeacher'])],
    children: [
      { path: 'profile', component: TeacherProfileComponent },
      { path: 'teaching-assignment', component: TeachingAssignmentComponent },
      { path: 'grade-entry', component: GradeEntryComponent },
      { path: 'grade-view', component: GradeViewComponent },
      { path: 'conduct-grading', component: ConductGradingComponent }
    ]
  },
  { path: '', redirectTo: '/login', pathMatch: 'full' }
];