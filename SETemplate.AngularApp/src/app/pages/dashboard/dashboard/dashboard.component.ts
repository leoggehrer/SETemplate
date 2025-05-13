import { Router } from '@angular/router';
import { Component } from '@angular/core';
import { AuthService } from '@app-services/auth.service';
import { environment } from '@environment/environment';

@Component({
  selector: 'app-dashboard',
  standalone: false,
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {
  public cards = [
    { title: 'Item1', text: 'Ein Text für das Item1', type: '/items1', bg: 'bg-primary text-white' },
    { title: 'Item2', text: 'Ein Text für das Item2', type: '/items2', bg: 'bg-success text-white' },
  ];

  constructor(
    private auth: AuthService, 
    private router: Router) {

  }

  public get requireLogin(): boolean {
    return environment.requireLogin;
  }

  public logout() {
    this.auth.logout();
    this.router.navigate(['/auth/login']);
  }
}
