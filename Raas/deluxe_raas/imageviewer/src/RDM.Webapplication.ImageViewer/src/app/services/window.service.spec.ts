import { TestBed, inject } from '@angular/core/testing';

import { WindowService } from './window.service';

import { Message } from '../vo/message';

import { MessageType } from '../constants/messageType';

describe('WindowService', () => {

    const path = 'viewimage/home?default=true&fullscreen=true';

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [] ,
            declarations: [],
            providers: [ WindowService ]
        }).compileComponents();
    });

    it('should create window service', inject([WindowService], (service: WindowService) => {
        expect(service).toBeTruthy();
    }));

    it('should open a new window', inject([WindowService], (service: WindowService) => {
        spyOn(window, 'open').and.callThrough();

        service.openFullScreen(path);

        expect(window.open).toHaveBeenCalledWith(`${window.location.origin}/${path}`, '_blank', `toolbar=0, top =0, left=0,width=${screen.availWidth},height=${screen.availHeight}`);
    }));

    it('should close window', inject([WindowService], (service: WindowService) => {
        spyOn(window, 'close').and.callThrough();

        service.closeWindow();

        expect(window.close).toHaveBeenCalled();
    }));

    it('should close child window', inject([WindowService], (service: WindowService) => {
        service.openFullScreen(path);

        spyOn(service.childWindow, 'close').and.callThrough();

        service.closeChildWindow();

        expect(service.childWindow.close).toHaveBeenCalled();
    }));

    it('should post a message to host', inject([WindowService], (service: WindowService) => {
        spyOn(window.parent, 'postMessage');

        const message = new Message(MessageType.CheckSession, null);

        service.openFullScreen(path);

        service.postMessageToHost(message);

        expect(window.parent.postMessage).toHaveBeenCalledWith(message, '*' as WindowPostMessageOptions);
    }));

    it('should post a message to parent window', inject([WindowService], (service: WindowService) => {
        const childWindow = window.open('', '', 'height=600, width=800');

        const message = new Message(MessageType.CheckSessionFromChild, null);

        window.opener = childWindow.opener;

        spyOn(window.opener, 'postMessage');

        service.postMessageToParent(message);

        expect(window.opener.postMessage).toHaveBeenCalledWith(message, window.opener.location.origin as WindowPostMessageOptions);
    }));

    it('should post a message to child window', inject([WindowService], (service: WindowService) => {
        service.openFullScreen(path);

        const message = new Message(MessageType.DocumentNameChange, 'doc name updated');

        spyOn(service.childWindow, 'postMessage');

        service.postMessageToChild(message);

        expect(service.childWindow.postMessage).toHaveBeenCalledWith(message, service.childWindow.location.origin as WindowPostMessageOptions);
    }));
});
