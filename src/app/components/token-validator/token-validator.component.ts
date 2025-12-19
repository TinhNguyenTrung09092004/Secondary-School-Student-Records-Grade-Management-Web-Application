import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-token-validator',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './token-validator.component.html'
})
export class TokenValidatorComponent implements OnInit {
  message = 'Đang xác thực...';
  error = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient
  ) {}

  ngOnInit() {
    const email = this.route.snapshot.queryParamMap.get('email');
    const token = this.route.snapshot.queryParamMap.get('token');
    const type = this.route.snapshot.queryParamMap.get('type') || 'reset';

    if (!email || !token) {
      this.error = 'Liên kết không hợp lệ';
      return;
    }

    // Store in sessionStorage and redirect without token in URL
    sessionStorage.setItem('pwd_email', email);
    sessionStorage.setItem('pwd_token', token);
    sessionStorage.setItem('pwd_type', type);

    // Clear URL immediately by redirecting
    if (type === 'setup') {
      this.router.navigate(['/setup-account'], { replaceUrl: true });
    } else {
      this.router.navigate(['/reset-password'], { replaceUrl: true });
    }
  }
}
