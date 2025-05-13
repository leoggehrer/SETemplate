//@CodeCopy
import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(private auth: AuthService, private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
    if (!environment.requireLogin || this.auth.isLoggedIn()) {
      return true;
    } else {
      return this.router.createUrlTree(['/auth/login'], { queryParams: { returnUrl: state.url } });
    }
  }
}
