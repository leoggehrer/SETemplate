//@BaseCode
import { Component } from '@angular/core';

import { FormsModule } from '@angular/forms';
import { AuthService } from '@app-services/auth.service';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, TranslateModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  // Whitelist of allowed redirect paths after login (prevent open redirect)
  private readonly allowedReturnPaths = ['/', '/dashboard'];

  public email = '';
  public password = '';
  public error = '';
  public returnUrl = '/';

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthService) {

  }

  ngOnInit() {
    const raw = this.route.snapshot.queryParams['returnUrl'] || '/';
    this.returnUrl = this.isSafeReturnUrl(raw) ? raw : '/';

    if (!this.authService.isLoginRequired || this.authService.isLoggedIn) {
      this.router.navigateByUrl(this.returnUrl);
    }
  }

  private isSafeReturnUrl(url: string): boolean {
    // Reject absolute URLs (external redirects) and only allow relative paths
    if (!url.startsWith('/') || url.startsWith('//')) {
      return false;
    }
    return this.allowedReturnPaths.some(p => url === p || url.startsWith(p + '/'));
  }

  public async onLogin() {

    try {
      const user = await this.authService.login(this.email, this.password);

      if (user) {
        localStorage.setItem('auth', 'true');
        this.router.navigateByUrl(this.returnUrl);
      }
      else {
        this.error = 'Login fehlgeschlagen';
      }
    }
    catch (error) {
      if (error instanceof HttpErrorResponse) {
        this.error = `Login error: ${error.status} ${error.statusText}\n${error.message}`;
      }
      else {
        this.error = 'Login error: An unknown error occurred.';
      }
    }
  }
}
