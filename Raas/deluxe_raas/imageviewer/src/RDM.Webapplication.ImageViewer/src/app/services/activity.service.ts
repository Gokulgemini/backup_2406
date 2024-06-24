import { Injectable } from '@angular/core';

import { WindowService } from './window.service';
import { ImageService } from './image.service';

import { MessageType } from '../constants/messageType';

import { Message } from '../vo/message';

@Injectable({
  providedIn: 'root'
})
export class ActivityService {

    constructor(private windowService: WindowService, private imageService: ImageService) { }

    logActivity() {
        if (this.imageService.isDefaultImageRequest) { return; }

        if (!this.windowService.isFullScreenMode) {
            this.windowService.postMessageToHost(new Message(MessageType.LogActivity, null));
        } else if (this.windowService.parentWindow !== null) {
            this.windowService.postMessageToParent(new Message(MessageType.LogActivityFromChild, null));
        }
    }
 }
