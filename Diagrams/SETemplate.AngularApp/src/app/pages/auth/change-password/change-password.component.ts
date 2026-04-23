//@BaseCode
import { Component } from '@angular/core';

import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '@app-services/auth.service';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [FormsModule, TranslateModule],
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.css'
})
export class ChangePasswordComponent {
  public oldPassword = '';
  public newPassword = '';
  public confirmPassword = '';
  public error = '';
  public success = '';
  public isLoading = false;

  constructor(
    private router: Router,
    private authService: AuthService
  ) {}

  public get passwordsMatch(): boolean {
    return this.newPassword === this.confirmPassword;
  }

  public get isFormValid(): boolean {
    return (
      this.oldPassword.length > 0 &&
      this.newPassword.length >= 6 &&
      this.passwordsMatch
    );
  }

  public async onChangePassword() {
    if (!this.isFormValid) return;

    this.error = '';
    this.success = '';
    this.isLoading = true;

    try {
      await this.authService.changePassword(this.oldPassword, this.newPassword);
      this.success = 'AUTH.CHANGE_PASSWORD.SUCCESS';
      this.oldPassword = '';
      this.newPassword = '';
      this.confirmPassword = '';
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        if (error.status === 401) {
          this.error = 'AUTH.CHANGE_PASSWORD.ERROR_WRONG_PASSWORD';
        } else {
          this.error = 'AUTH.CHANGE_PASSWORD.ERROR_GENERIC';
        }
      } else {
        this.error = 'AUTH.CHANGE_PASSWORD.ERROR_GENERIC';
      }
    } finally {
      this.isLoading = false;
    }
  }

  public cancel() {
    this.router.navigateByUrl('/dashboard');
  }
}
