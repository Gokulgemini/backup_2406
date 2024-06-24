import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

import { ImageService } from './image.service';
import { WindowService } from './window.service';

import { ErrorType } from '../constants/errorType';
import { MessageType } from '../constants/messageType';

import { Message } from '../vo/message';

@Injectable({
  providedIn: 'root'
})
export class SessionService {

  constructor(private windowService: WindowService,
              private route: Router,
              private imageService: ImageService) { }

  checkSessionStatus() {

    if (this.imageService.isDefaultImageRequest) { return; }

    if (!this.windowService.isFullScreenMode) {
      this.windowService.postMessageToHost(new Message(MessageType.CheckSession, null));
    } else {

      if (this.windowService.parentWindow === null) {
        this.route.navigate(['/error-page', ErrorType.Unauthorized]);
      }

      this.windowService.postMessageToParent(new Message(MessageType.CheckSessionFromChild, null));
    }
  }
}
