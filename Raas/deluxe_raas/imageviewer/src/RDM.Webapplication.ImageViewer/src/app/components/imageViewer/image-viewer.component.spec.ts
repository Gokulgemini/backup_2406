import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSliderModule } from '@angular/material/slider';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AngularDraggableModule } from 'angular2-draggable';
import { ActivatedRoute } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { ModalModule } from 'ngx-bootstrap/modal';
import { FormsModule } from '@angular/forms';
import { ToastrModule } from 'ngx-toastr';
import { TruncateModule } from '@yellowspot/ng-truncate';

import { CookieService } from "ngx-cookie-service";
import { ImageService } from '../../services/image.service';
import { WindowService } from '../../services/window.service';
import { ConfigService } from '../../services/config.service';

import { ToolbarComponent } from '../toolbar/toolbar.component';
import { ToolbarButtonsComponent } from './../toolbar-buttons/toolbar-buttons.component';
import { ToolbarPaginationComponent } from './../toolbar-pagination/toolbar-pagination.component';
import { ToolbarDocumentNameComponent } from './../toolbar-document-name/toolbar-document-name.component';
import { ImageViewerComponent } from './image-viewer.component';

import { ActivatedRouteStub } from '../../stubs/activateRoute.stub';
import { ImageServiceStub } from '../../stubs/image.service.stub';
import { ConfigServiceStub } from '../../stubs/config.service.stub';

import { ImageType } from '../../constants/imageType';
import { ImageDefault } from '../../constants/imageDefault';

describe('ImageViewerComponent', () => {

    let component: ImageViewerComponent;
    let fixture: ComponentFixture<ImageViewerComponent>;

    let imageService: ImageServiceStub;
    let windowService: WindowService;
    let activatedRoute: ActivatedRouteStub;
    let configService: ConfigServiceStub;

    beforeEach(() => {

        imageService = new ImageServiceStub();
        windowService = new WindowService;
        activatedRoute = new ActivatedRouteStub();
        configService = new ConfigServiceStub();

        TestBed.configureTestingModule({
            imports: [
                RouterTestingModule,
                FormsModule,
                AngularDraggableModule,
                BrowserAnimationsModule,
                HttpClientModule,
                MatSliderModule,
                MatButtonModule,
                MatToolbarModule,
                MatIconModule,
                MatTooltipModule,
                MatProgressSpinnerModule,
                TruncateModule,
                ModalModule.forRoot(),
                ToastrModule.forRoot()
            ],
            declarations: [
                ImageViewerComponent,
                ToolbarComponent,
                ToolbarDocumentNameComponent,
                ToolbarPaginationComponent,
                ToolbarButtonsComponent
            ],
            providers: [
                WindowService,
                CookieService,
                { provide: ConfigService, useValue: configService },
                { provide: ImageService, useValue: imageService },
                { provide: ActivatedRoute, useValue: activatedRoute }
            ]
        }).compileComponents();

        fixture = TestBed.createComponent(ImageViewerComponent);
        component = fixture.componentInstance;

        activatedRoute = fixture.debugElement.injector.get(ActivatedRoute) as any;
        activatedRoute.testParamMap = { irn: 'ABCDEFGHIJKLMN' };
        activatedRoute.testParamMap = { type: ImageType.Check };
        activatedRoute.testQueryParamMap = { fullscreen: null };

        component.isLoading = true;

        fixture.detectChanges();
    });

    it('should adjust slider value when zooming in with scroll', () => {

        for (let i = 1; i < 5.5; i += 0.1) {
            component.sliderValue = i;
            component.scrollZoom('in');

            if (i >= 4.9) {
                expect(component.sliderValue).toEqual(5);
            } else {
                expect(component.sliderValue).toEqual( i + 0.1);
            }
        }
    });

    it('should adjust slider value when zooming out with scroll', () => {

        for (let i = 1; i >= -1; i -= 0.1) {
            component.sliderValue = i;
            component.scrollZoom('out');

            if (i <= 0.2) {
                expect(component.sliderValue).toEqual(0.1);
            } else {
                expect(component.sliderValue).toEqual( i - 0.1);
            }
        }
    });

    it('should get dynamic properties of image', () => {
        imageService.imageConfig.ImageWidth = 1000;
        component.imgWidth_InitialSize = null;
        component.checkImage.nativeElement.height = 1000;
        component.sliderValue = 1.2;
        component.wasDragged = true;
        component.imgWidthProportionsToVerticalAxis = null;

        component.sliderZoom(component.sliderValue);

        expect(component.imgWidth_After_Zoom).toEqual(1200);
        expect(component.imgHeight_After_Zoom).toEqual(1200);
        expect(component.imgWidth_Before_Zoom).toEqual(1000);
        expect(component.imgHeight_Before_Zoom).toEqual(1000);
        expect(imageService.imageConfig.ImageWidth).toEqual(component.imgWidth_After_Zoom);
    });

    it('should get the image proportions with regards to mouse position', () => {
        component.mousePosX = 500;
        component.mousePosY = 900;
        component.img_posX = 200;
        component.img_posY = 300;
        imageService.imageConfig.ImageWidth = 1000;
        component.wasDragged = true;
        component.checkImage.nativeElement.height = 1000;

        component.scrollZoom('in');

        expect(component.imgWidthProportionsToVerticalAxis).toEqual(0.3);
        expect(component.imgHeightProportionsToHorizontalAxis).toEqual(0.6);
    });

    it('should get the image proportions with regards to center screen', () => {
        component.img_posX = 200;
        component.img_posY = 300;
        imageService.imageConfig.ImageWidth = 1000;
        component.wasDragged = true;
        component.checkImage.nativeElement.height = 1000;
        component.sliderZoom(0.2);

        const correctWidthRatio = (((windowService.nativeWindow.innerWidth / 2) - 200) / 200);
        const correctHeightRatio = (((windowService.nativeWindow.innerHeight / 2) - 300) / 200);

        expect(component.imgWidthProportionsToVerticalAxis).toEqual(correctWidthRatio);
        expect(component.imgHeightProportionsToHorizontalAxis).toEqual(correctHeightRatio);
    });

    it('should get mouse coordinates', () => {

        const event = new MouseEvent('dxcontextmenu', 
        { 
            bubbles: true, 
            screenX: 10 , 
            screenY: 10 , 
            clientX: 10 , 
            clientY: 10 ,
            ctrlKey: true ,
            shiftKey: true ,
            altKey: true ,
            metaKey: true ,
            button: 1 ,
            relatedTarget: null
        });

        component.getMouseCoords(event);

        expect(component.mousePosX ).toEqual(event.clientX);
        expect(component.mousePosY ).toEqual(event.clientY);
    });

    it('should get screen coordinates', () => {

        const event = {x: 1, y: 1};

        component.onMove(event);

        expect(imageService.imageConfig.ImagePosition.x ).toEqual(event.x);
        expect(imageService.imageConfig.ImagePosition.y ).toEqual(event.y);
    });

    it('should load a remittance image', () => {

        activatedRoute.testParamMap = { type: ImageType.Remittance };

        component.ngOnInit();

        expect(imageService.imageConfig.ImageType).toBe(ImageType.Remittance);
    });

    it('should load a default image', fakeAsync(() => {

        activatedRoute.testParamMap = { type: ImageType.GeneralDocument };
        imageService.displayDefaultImage = true;

        component.ngOnInit();

        tick(1000);

        expect(imageService.imageConfig.ImageType).toBe(ImageDefault.Type);
        expect(imageService.imageConfig.ImageFrontContent).toBe(ImageDefault.FrontImage.Content);
        expect(imageService.imageConfig.ImageBackContent).toBe(ImageDefault.BackImage.Content);
    }));

    it('should load a scanned image', () => {

        activatedRoute.testParamMap = { type: ImageType.GeneralDocument };
        imageService.displayDefaultImage = true;
        imageService.displayScannedImage = true;

        component.ngOnInit();

        expect(imageService.imageConfig.ImageType).toBe(ImageDefault.Type);
        expect(imageService.imageConfig.ImageFrontContent).toBe(imageService.imageSourceCheck);
    });

    it('should be notified when image shape changes', fakeAsync(() => {

        imageService.notifyWhenImageShapeChanged();

        tick();

        expect(component.sliderValue).toBe(1);
        expect(component.imgWidth_InitialSize).toBeNull();
    }));

    it('should be notified when general document page index changes', fakeAsync(() => {
        activatedRoute.testParamMap = { type: ImageType.GeneralDocument };

        component.ngOnInit();

        imageService.imageConfig.ImagePage = 2;

        const index = imageService.imageConfig.ImagePage - 1;

        imageService.notifyWhenPageChanged();

        tick();

        expect(imageService.imageConfig.ImageSource).toContain(component.imageDocument.Pages[index].FrontImage.Content);
    }));

    it('should be notified when spinner state need to be changed', fakeAsync(() => {
        component.isLoading = false;

        imageService.notifyLoadImageStatus(true);

        tick();

        expect(component.isLoading).toBeTruthy();
    }));
});
