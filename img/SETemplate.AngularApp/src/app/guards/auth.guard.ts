//@CodeCopy
import { Injectable } from '@angular/core';
import { AuthService } from '@app-services/auth.service';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {

  constructor(
    private router: Router,
    private authService: AuthService) {

  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Promise<boolean | UrlTree> {
    return this.authService.isSessionAlive().then((isAlive) => {
      if (!isAlive) {
        this.authService.resetUser();

        const returnUrl = route.routeConfig?.path ?? '/auth/login';

        return this.router.createUrlTree(['/auth/login'], {
          queryParams: { returnUrl }
        });
      }

      return true;
    });
  }
}
