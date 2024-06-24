import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CookieService } from "ngx-cookie-service";

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

    constructor(private cookieService: CookieService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        
        const access_token = this.cookieService.get('access_token');
        const su_token = this.cookieService.get('su_token') || '';

        request = request.clone({
            setHeaders: {
                Authorization: 'Bearer ' + access_token,
                ApplicationName: 'ImageViewer',
                ApplicationVersion: '2',
                SUToken: su_token
            }
        });

        return next.handle(request);
    }
}
