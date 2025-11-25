//@CodeCopy
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { MessageBoxService } from '@app-services/message-box-service.service';
import { ErrorHandlerService } from '@app-services/error-handler.service';

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {
  constructor(
    private messageBoxService: MessageBoxService,
    private errorHandlerService: ErrorHandlerService,
    private router: Router
  ) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = '';
        let errorTitle = 'ERRORS.GENERAL';
        
        if (error.error instanceof ErrorEvent) {
          // Client-seitiger Fehler
          errorMessage = `Client Error: ${error.error.message}`;
          errorTitle = 'ERRORS.NETWORK';
        } else {
          // Server-seitiger Fehler
          switch (error.status) {
            case 401:
              errorTitle = 'ERRORS.UNAUTHORIZED';
              errorMessage = 'Sie sind nicht angemeldet oder Ihre Sitzung ist abgelaufen.';
              // Zur Login-Seite umleiten
              setTimeout(() => {
                this.router.navigate(['/auth/login']);
              }, 2000);
              break;
            case 403:
              errorTitle = 'ERRORS.FORBIDDEN';
              errorMessage = 'Sie haben keine Berechtigung für diese Aktion.';
              break;
            case 404:
              errorTitle = 'ERRORS.NOT_FOUND';
              errorMessage = 'Die angeforderte Ressource wurde nicht gefunden.';
              break;
            case 500:
              errorTitle = 'ERRORS.SERVER_ERROR';
              errorMessage = 'Ein Serverfehler ist aufgetreten.';
              break;
            default:
              errorTitle = 'ERRORS.GENERAL';
              errorMessage = `Fehler ${error.status}: ${error.statusText}`;
          }
          
          // Detaillierte Fehlermeldung vom ErrorHandlerService
          const details = this.errorHandlerService.extractErrorDetails(error);
          
          // Zeige MessageBox mit Fehlerdetails
          this.messageBoxService.show(errorMessage, errorTitle, 'OK', details);
        }
        
        // Fehler weitergeben für spezifische Behandlung in Komponenten
        return throwError(() => error);
      })
    );
  }
}
