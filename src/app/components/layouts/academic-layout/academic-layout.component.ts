import { Component, inject } from '@angular/core';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { filter } from 'rxjs/operators';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-academic-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './academic-layout.component.html',
  styleUrl: './academic-layout.component.css'
})
export class AcademicLayoutComponent {
  showDashboard = true;
  isPrincipal = false;
  isAcademicAffairs = false;
  isDepartmentHead = false;
  private authService = inject(AuthService);

  constructor(private router: Router) {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      this.showDashboard = event.url === '/academic';
    });

    this.authService.currentUser$.subscribe(user => {
      this.isPrincipal = user?.roles.includes('Principal') || false;
      this.isAcademicAffairs = user?.roles.includes('AcademicAffairs') || false;
      this.isDepartmentHead = user?.roles.includes('DepartmentHead') || false;
    });
  }

  logout(event: Event) {
    event.preventDefault();
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }
}
