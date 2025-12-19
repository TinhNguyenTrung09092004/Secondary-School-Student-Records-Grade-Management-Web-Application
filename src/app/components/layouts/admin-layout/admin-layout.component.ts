import { Component } from '@angular/core';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.css'
})
export class AdminLayoutComponent {
  showDashboard = true;

  constructor(private router: Router) {
    // Show dashboard only when on /admin (no child route)
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      this.showDashboard = event.url === '/admin';
    });
  }

  logout(event: Event) {
    event.preventDefault();
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }
}
