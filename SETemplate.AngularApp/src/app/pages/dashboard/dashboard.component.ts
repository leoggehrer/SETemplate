import { Router } from '@angular/router';
import { Component } from '@angular/core';
import { AuthService } from '@app-services/auth.service';

export class DashboardCard {
  title: string;
  text: string;
  type: string;
  bg: string;
  icon: string;
  constructor(title: string, text: string, type: string, bg: string, icon: string) {
    this.title = title;
    this.text = text;
    this.type = type;
    this.bg = bg;
    this.icon = icon;
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
//    { title: 'DASHBOARD.CARDS.DASHBOARD_TITLE', text: 'DASHBOARD.CARDS.DASHBOARD_TEXT', type: '/dashboard', bg: 'bg-primary text-white', icon: 'bi-speedometer2' },
  ];

  public authCards: DashboardCard[] = [
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
