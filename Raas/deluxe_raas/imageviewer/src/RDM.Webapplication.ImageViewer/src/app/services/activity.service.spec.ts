import { TestBed, inject } from '@angular/core/testing';

import { WindowService } from './window.service';
import { ImageService } from './image.service';
import { ActivityService } from './activity.service';

import { ImageServiceStub } from '../stubs/image.service.stub';
import { WindowServiceStub } from '../stubs/window.service.stub';

import { Message } from '../vo/message';

import { MessageType } from '../constants/messageType';

describe('ActivityService', () => {

    let windowService: WindowServiceStub;
    let imageService: ImageServiceStub;

    beforeEach(() => {

        windowService = new WindowServiceStub();
        imageService = new ImageServiceStub();

        TestBed.configureTestingModule({
            imports: [] ,
            declarations: [],
            providers: [
                ActivityService,
                { provide: WindowService, useValue: windowService },
                { provide: ImageService, useValue: imageService }
             ]
        }).compileComponents();
    });

    it('should create window service', inject([ActivityService], (service: ActivityService) => {
        expect(service).toBeTruthy();
    }));

    it('should trigger log activity when it is NOT full screen mode', inject([ActivityService], (service: ActivityService) => {
        spyOn(windowService, 'postMessageToHost');

        windowService.isFullscreenMode = false;
        imageService.displayDefaultImage = false;

        const message = new Message(MessageType.LogActivity, null);

        service.logActivity();

        expect(windowService.postMessageToHost).toHaveBeenCalledWith(message);
    }));

    it('should trigger log activity when it is full screen mode', inject([ActivityService], (service: ActivityService) => {
        spyOn(windowService, 'postMessageToParent');

        windowService.isFullscreenMode = true;
        imageService.displayDefaultImage = false;

        const message = new Message(MessageType.LogActivityFromChild, null);

        service.logActivity();

        expect(windowService.postMessageToParent).toHaveBeenCalledWith(message);
    }));

    it('should NOT trigger log activity when default image is loaded', inject([ActivityService], (service: ActivityService) => {
        spyOn(windowService, 'postMessageToHost');
        spyOn(windowService, 'postMessageToParent');

        imageService.displayDefaultImage = true;

        service.logActivity();

        expect(windowService.postMessageToHost).not.toHaveBeenCalled();
        expect(windowService.postMessageToParent).not.toHaveBeenCalled();
    }));
});
