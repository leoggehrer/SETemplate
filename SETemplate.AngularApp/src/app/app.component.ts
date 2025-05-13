import { Component } from '@angular/core';
import { AuthService } from './services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'SETemplate-Developer';

    constructor(
      private authService: AuthService, 
      private router: Router) { 

    }

  logout() {
    this.authService.logout();
    this.router.navigate(['/auth/login']);

  }

}
