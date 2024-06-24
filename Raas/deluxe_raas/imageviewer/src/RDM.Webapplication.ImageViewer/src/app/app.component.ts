import { Component, OnInit, Renderer2, HostListener, isDevMode  } from '@angular/core';
import { Router } from '@angular/router';

import { WindowService } from './services/window.service';
import { SessionService } from './services/session.service';
import { ActivityService } from './services/activity.service';
import { ImageService } from './services/image.service';
import { TokenService } from './services/token.service';

import { ErrorType } from './constants/errorType';
import { MessageType } from './constants/messageType';

import { Message } from './vo/message';

@Component({
  selector: 'app-root',
  template: '<home></home>'
})
export class AppComponent implements OnInit {

  constructor(private renderer: Renderer2,
              private route: Router,
              private windowService: WindowService,
              private sessionService: SessionService,
              private activityService: ActivityService,
              private imageService: ImageService,
              private tokenService: TokenService) {}

  ngOnInit() {
    if (!isDevMode()) {
      this.renderer.listen('window', 'message', this.handleMessage.bind(this));
    }
  }

  @HostListener('window:beforeunload', ['$event']) beforeUnloadHandler(event: Event) {
    this.windowService.closeChildWindow();
  }

  @HostListener('window:unload', ['$event']) unloadHandler(event: Event) {
    this.windowService.closeChildWindow();
  }

  @HostListener('window:keydown.escape', ['$event']) onEscapeHandler(event: Event) {
    this.windowService.closeWindow();
  }

  private handleMessage(event: Event) {
    const messageEvent = event as MessageEvent;

    const message = (messageEvent.data as Message);

    switch (message.Type) {

      // Message from full screen window (child window) to check ITMS sessions
      case MessageType.CheckSessionFromChild:
        this.sessionService.checkSessionStatus();
      break;

      // Message from parent window, document name has changed
      case MessageType.DocumentNameChange:
        this.imageService.notifyChildrenWhenDocNameChanged(message.Value);
      break;

      // Message from full screen window (child window) to log activity
      case MessageType.LogActivityFromChild:
        this.activityService.logActivity();
      break;

      // Message from ITMS Host, whether session is expired or not
      case MessageType.SessionHasExpired:
        if (message.Value) {
          this.windowService.closeChildWindow();
          this.route.navigate(['/error-page', ErrorType.Unauthorized]);
        }
      break;

      // Message from ITMS Host, whether token is expired or not
      case MessageType.TokenHasExpired:
        if (message.Value) {
          this.route.navigate(['/error-page', ErrorType.Unauthorized]);
        } else {
          this.tokenService.notifyWhenTokenIsValid();
        }
      break;

      default:
      break;
    }
  }
}
