import { Router } from '@angular/router';
import { Component } from '@angular/core';
import { AuthService } from '@app-services/auth.service';

export class DashboardCard {
  visible: boolean = true;
  title: string;
  text: string;
  type: string;
  bg: string;
  icon: string;
  constructor(visible: boolean = true, title: string, text: string, type: string, bg: string, icon: string) {
    this.visible = visible;
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
//    { visible: true, title: 'DASHBOARD.CARDS.DASHBOARD_TITLE', text: 'DASHBOARD.CARDS.DASHBOARD_TEXT', type: '/dashboard', bg: 'bg-primary text-white', icon: 'bi-speedometer2' },
  ];

  public authCards: DashboardCard[] = [
  ];

  constructor(
    private authService: AuthService,
    private router: Router) {

  }

  public get isLoginRequired(): boolean {
    return this.authService.isLoginRequired;
  }

  public get isLoggedIn(): boolean {
    return this.authService.isLoggedIn;
  }

  public get visiblePublicCards(): DashboardCard[] {
    return this.publicCards.filter(c => c.visible);
  }

  public get visibleAuthCards(): DashboardCard[] {
    return this.authCards.filter(c => c.visible);
  }

  public logout() {
    this.authService.logout();
  }
}
