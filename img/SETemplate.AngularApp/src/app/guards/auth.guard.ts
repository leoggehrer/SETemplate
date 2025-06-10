//@CodeCopy
import { Injectable } from '@angular/core';
import { environment } from '@environment/environment';
import { AuthService } from '@app-services/auth.service';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {

  constructor(
    private router: Router,
    private authService: AuthService
  ) { }

  async canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Promise<boolean | UrlTree> {

    if (environment.loginRequired === false) {
      return true;
    }

    const isAlive = await this.authService.isSessionAlive();

    if (!isAlive) {
      this.authService.resetUser();

      return this.router.createUrlTree(['/auth/login'], {
        queryParams: { returnUrl: state.url }
      });
    }

    return true;
  }
}
