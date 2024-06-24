import { Message } from '../vo/message';

export class WindowServiceStub {

    isFullscreenMode = false;

    get isFullScreenMode(): boolean {
        return this.isFullscreenMode;
    }

    get nativeWindow(): Window {
        return window;
     }

    openFullScreen(path: string) {}
    closeWindow() {}
    postMessageToHost(message: Message) {}
    postMessageToParent(message: Message) {}
    postMessageToChild(message: Message) {}
}
