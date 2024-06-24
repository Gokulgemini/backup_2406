import { Message } from '../vo/message';
import { Injectable } from "@angular/core";

@Injectable()
export class WindowService {

   childWindow: Window;

   get nativeWindow(): Window {
      return window;
   }

   get isFullScreenMode(): boolean {
      return window === top;
   }

   get parentWindow(): any {
      return window.opener;
   }

   postMessageToHost(message: Message) {
      if (!window.parent) { return; }

      window.parent.postMessage(message, '*');
   }

   postMessageToParent(message: Message) {
      if (!this.parentWindow) { return; }

      this.parentWindow.postMessage(message, this.parentWindow.location.origin);
   }

   postMessageToChild(message: Message) {
      if (!this.childWindow) { return; }

      this.childWindow.postMessage(message, this.childWindow.location.origin);
   }

   closeWindow() {
      window.close();
   }

   openFullScreen(path: string) {
      this.closeChildWindow();

      this.childWindow = window.open(`${window.location.origin}/${path}`, '_blank', `toolbar=0, top =0, left=0,width=${screen.availWidth},height=${screen.availHeight}`);

      this.childWindow.onkeydown = evt => {
         if (evt.key === 'Escape') {
            evt.stopPropagation();
         }
      };
   }

   closeChildWindow() {
      if (!this.childWindow) { return; }

      this.childWindow.close();
   }
}
