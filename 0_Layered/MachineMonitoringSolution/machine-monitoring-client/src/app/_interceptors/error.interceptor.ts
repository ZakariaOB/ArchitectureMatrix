import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { Router, NavigationExtras } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private toastr: ToastrService) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error) => {
        if (error) {
          switch (error.status) {
            case 400:
              if (error.error.errors) {
                const modalStateErrors = [];
                for (const key in error.error.errors) {
                  if (error.error.errors[key]) {
                    modalStateErrors.push(error.error.errors[key]);
                  }
                }
                throw modalStateErrors.flat();
              } else if (typeof error.error === 'object') {
                this.toastr.error(error.statusText, error.status);
              } else {
                this.toastr.error(error.error, error.status);
              }
              break;
            case 401:
            case 500:
              this.toastr.error(error.error);
              break;
            default:
              this.toastr.error('Something unexpected went wrong');
              console.log(error);
              break;
          }
        }
        return throwError(() => error);
      })
    );
  }
}
