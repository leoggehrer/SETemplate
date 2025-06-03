import { Router } from '@angular/router';
import { Component } from '@angular/core';
import { AuthService } from '@app-services/auth.service';

export class DashboardCard {
  title: string;
  text: string;
  type: string;
  bg: string;
  constructor(title: string, text: string, type: string, bg: string) {
    this.title = title;
    this.text = text;
    this.type = type;
    this.bg = bg;
  }
}

@Component({
  selector: 'app-dashboard',
  standalone: false,
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {

  public publicCards: DashboardCard[] = [
    { title: 'Dashboard', text: 'Übersicht', type: '/dashboard', bg: 'bg-primary text-white' },
  ];

  public authCards: DashboardCard[] = [
  ];

  public statisticCards: DashboardCard[] = [
  ];

  constructor(
    private authService: AuthService,
    private router: Router) {

  }

  public get isLoginReqired(): boolean {
    return this.authService.isLoginRequired;
  }

  public get isLoggedIn(): boolean {
    return this.authService.isLoggedIn;
  }

  public logout() {
    this.authService.logout();
  }
}
