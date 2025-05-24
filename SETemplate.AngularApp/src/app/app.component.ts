import { Component } from '@angular/core';
import { AuthService } from './services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: false,
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  public title = 'SEMusicStoreAngular-Developer';

  public get isLoginRequired(): boolean {
    return this.authService.isLoginRequired;
  }
  public get isLoggedIn(): boolean {
    return this.authService.isLoggedIn;
  }

  constructor(
    private router: Router,
    private authService: AuthService) {

  }

  public logout() {
    this.authService.logout();
    this.router.navigate(['/dashboard']);
  }
}
