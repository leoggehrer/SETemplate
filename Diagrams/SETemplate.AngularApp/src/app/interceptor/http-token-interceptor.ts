//@CodeCopy
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from '@app-services/auth.service';
import { Observable } from 'rxjs';

@Injectable()
export class HttpTokenInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {
  }

  intercept(request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    request = this.addToken(request);
    return next.handle(request);
  }

  private addToken(request: HttpRequest<any>) {
    var sessionToken;

    if (this.authService.user && this.authService.user.sessionToken) {
      sessionToken = this.authService.user.sessionToken;
    }

    var authHeaderValue;

    if (sessionToken) {
      authHeaderValue = 'SessionToken ' + sessionToken;
    }

    if (authHeaderValue) {
      const requestCopy = request.clone({
        headers: request.headers,
        setHeaders: {
          Authorization: authHeaderValue,
        },
      });

      return requestCopy;
    }
    else {
      return request;
    }
  }
}
