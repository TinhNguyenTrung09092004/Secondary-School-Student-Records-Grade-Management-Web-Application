import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { filter } from 'rxjs/operators';
import { AuthService } from '../../../services/auth.service';
import { TeachingAssignmentService } from '../../../services/teachingassignment.service';

@Component({
  selector: 'app-teacher-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './teacher-layout.component.html'//,
  //styleUrl: './teacher-layout.component.css'
})
export class TeacherLayoutComponent implements OnInit {
  showDashboard = true;
  isDepartmentHead = false;
  private authService = inject(AuthService);
  private teachingAssignmentService = inject(TeachingAssignmentService);

  constructor(private router: Router) {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      this.showDashboard = event.url === '/teacher';
    });
  }

  ngOnInit(): void {
    this.checkDepartmentHead();
  }

  checkDepartmentHead(): void {
    this.teachingAssignmentService.checkIsDepartmentHead().subscribe({
      next: (response) => {
        this.isDepartmentHead = response.isDepartmentHead;
      },
      error: (error) => {
        this.isDepartmentHead = false;
      }
    });
  }

  logout(event: Event) {
    event.preventDefault();
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }
}
