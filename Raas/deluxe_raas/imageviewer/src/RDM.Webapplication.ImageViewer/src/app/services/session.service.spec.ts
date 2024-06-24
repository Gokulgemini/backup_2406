import { TestBed, inject } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { WindowService } from './window.service';
import { SessionService } from './session.service';
import { ImageService } from './image.service';

import { ImageServiceStub } from '../stubs/image.service.stub';
import { WindowServiceStub } from '../stubs/window.service.stub';

describe('SessionService', () => {

    let windowService: WindowServiceStub;
    let imageService: ImageServiceStub;

    beforeEach(() => {

        windowService = new WindowServiceStub();
        imageService = new ImageServiceStub();

        TestBed.configureTestingModule({
            imports: [RouterTestingModule] ,
            declarations: [],
            providers: [
                SessionService,
                { provide: WindowService, useValue: windowService },
                { provide: ImageService, useValue: imageService }
             ]
        }).compileComponents();
    });

    it('should create window service', inject([SessionService], (service: SessionService) => {
        expect(service).toBeTruthy();
    }));

    it('should post a message to Host when it is NOT full screen mode', inject([SessionService], (service: SessionService) => {
        spyOn(windowService, 'postMessageToHost');

        service.checkSessionStatus();

        expect(windowService.postMessageToHost).toHaveBeenCalled();
    }));

    it('should post a message to parent when it is full screen mode', inject([SessionService], (service: SessionService) => {
        spyOn(windowService, 'postMessageToParent');

        windowService.isFullscreenMode = true;

        service.checkSessionStatus();

        expect(windowService.postMessageToParent).toHaveBeenCalled();
    }));

    it('should post a message to parent when it is full screen mode', inject([SessionService], (service: SessionService) => {
        spyOn(windowService, 'postMessageToParent');
        spyOn(windowService, 'postMessageToHost');

        imageService.displayDefaultImage = true;

        service.checkSessionStatus();

        expect(windowService.postMessageToParent).not.toHaveBeenCalled();
        expect(windowService.postMessageToHost).not.toHaveBeenCalled();
    }));
});
