import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { TokenService } from '../services/token.service';

import { ErrorType } from '../constants/errorType';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(private tokenService: TokenService, private route: Router) {}

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        return next.handle(request).pipe(
            catchError(err => {

                console.log('Status: ' + err.status);
                console.log('Error: ' + err);

                this.tokenService.clearTokens();

                let errorType: any;

                switch (err.status) {
                    case 404:
                        errorType = ErrorType.PageNotFound;
                    break;

                    case 401:
                    case 403:
                        errorType = ErrorType.Unauthorized;
                    break;

                    default:
                        errorType = ErrorType.InternalServer;
                    break;
                }

                this.route.navigate(['/error-page', errorType]);

                const error = err.error || err.statusText;
                return throwError(error);
            })
        );
    }
}
