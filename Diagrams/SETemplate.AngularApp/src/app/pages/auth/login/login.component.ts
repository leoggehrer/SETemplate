//@CodeCopy
import { Component } from '@angular/core';
import { AuthService } from '@app-services/auth.service';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
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
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';

    if (!this.authService.isLoginRequired || this.authService.isLoggedIn) {
      this.router.navigateByUrl(this.returnUrl);
    }
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
