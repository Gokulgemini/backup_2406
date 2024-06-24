import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

import { Subject, Observable } from 'rxjs';

import { Message } from '../vo/message';

import { StringUtils } from '../utils/string.utils';

import { ErrorType } from '../constants/errorType';
import { MessageType } from '../constants/messageType';

import { CookieService } from "ngx-cookie-service";
import { WindowService } from './window.service';
import { ImageService } from './image.service';

@Injectable({
    providedIn: 'root'
  })
  
export class TokenService {

    private notifyTokenIsValid = new Subject<boolean>();

    notifyTokenIsValid$: Observable<boolean>;

    constructor(private windowService: WindowService,
                private route: Router,
                private imageService: ImageService,
                private cookieService: CookieService) {
            this.notifyTokenIsValid$ = this.notifyTokenIsValid.asObservable();
        }

    notifyWhenTokenIsValid() {
        this.notifyTokenIsValid.next(true);
    }

    checkTokenStatus() {
        if (this.imageService.isDefaultImageRequest || this.windowService.isFullScreenMode) { return; }

        this.windowService.postMessageToHost(new Message(MessageType.CheckToken, null));
      }

    storeSUToken(token: string) {
        if (!StringUtils.isNullOrEmpty(token)) {
            this.cookieService.set('su_token', token);
            return;
        }

        this.clearTokens();
        this.route.navigate(['/error-page', ErrorType.Unauthorized]);
    }

    removeSUToken() {
        this.cookieService.delete('su_token');
    }

    clearTokens() {
        this.cookieService.deleteAll();
    }
}
