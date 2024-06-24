import { ComponentFixture, TestBed, fakeAsync, tick, inject, discardPeriodicTasks } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { ModalModule } from 'ngx-bootstrap/modal';
import { ActivatedRoute } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';

import { ToolbarButtonsComponent } from './toolbar-buttons.component';

import { ImageFace } from '../../constants/imageFace';
import { ImageType } from '../../constants/imageType';
import { TargetHost } from '../../constants/targetHost';

import { CookieService } from "ngx-cookie-service";
import { WindowService } from '../../services/window.service';
import { ImageService } from '../../services/image.service';
import { SessionService } from '../../services/session.service';
import { TokenService } from '../../services/token.service';

import { ImageServiceStub } from '../../stubs/image.service.stub';
import { ActivatedRouteStub } from './../../stubs/activateRoute.stub';
import { SessionServiceStub } from './../../stubs/session.service.stub';
import { WindowServiceStub } from './../../stubs/window.service.stub';

import { GeneralDocument } from './../../models/generalDocument';
import { Check } from './../../models/check';

describe('ToolbarButtonsComponent', () => {
  let component: ToolbarButtonsComponent;
  let fixture: ComponentFixture<ToolbarButtonsComponent>;

  let imageService: ImageServiceStub;
  let activatedRoute: ActivatedRouteStub;
  let sessionService: SessionServiceStub;
  let windowService: WindowServiceStub;

  beforeEach(() => {

    imageService = new ImageServiceStub();
    activatedRoute = new ActivatedRouteStub();
    sessionService = new SessionServiceStub();
    windowService = new WindowServiceStub();

    TestBed.configureTestingModule({
        imports: [
          RouterTestingModule,
          ModalModule.forRoot()
        ],
        declarations: [
          ToolbarButtonsComponent
        ],
        providers: [
            TokenService,
            CookieService,
            { provide: WindowService, useValue: windowService },
            { provide: SessionService, useValue: sessionService },
            { provide: ImageService, useValue: imageService },
            { provide: ActivatedRoute, useValue: activatedRoute }
        ]
    }).compileComponents();

    fixture = TestBed.createComponent(ToolbarButtonsComponent);
    component = fixture.componentInstance;

    activatedRoute = fixture.debugElement.injector.get(ActivatedRoute) as any;
    activatedRoute.testParamMap = { type: ImageType.Check };
    activatedRoute.testQueryParamMap = { fullscreen: true };

    fixture.detectChanges();
  });

  it('should print image', () => {
    spyOn(component, 'print').and.callThrough();
    spyOn(windowService.nativeWindow, 'print');

    const image = {
      FrontImage: { Content: imageService.imageSourceCheck, Width: 1600, Height: 1200 },
      BackImage: { Content: '', Width: 0, Height: 0 }
    };

    const document = new Check((image as Check).FrontImage, (image as Check).BackImage);

    imageService.imageConfig.ImageType = ImageType.Check;
    imageService.setImageConfig(document);

    fixture.detectChanges();

    const dePrint = fixture.debugElement.query(By.css('#print'));
    const btnPrint: HTMLElement = dePrint.nativeElement;

    btnPrint.click();

    expect(component.print).toHaveBeenCalled();
    expect(windowService.nativeWindow.print).toHaveBeenCalled();
  });

  it('should NOT print image if image is not loaded', () => {
    spyOn(windowService.nativeWindow, 'print');

    component.imageService.imageConfig.ImageSource = null;

    component.print();

    expect(windowService.nativeWindow.print).not.toHaveBeenCalled();
  });

  it('should open a confirm box', () => {
    spyOn(component, 'openDialogBox').and.callThrough();

    const deOpenDialog = fixture.debugElement.query(By.css('#openDialog'));
    const btnOpenDialog: HTMLElement = deOpenDialog.nativeElement;

    btnOpenDialog.click();

    expect(component.openDialogBox).toHaveBeenCalled();
  });

  it('should close a confirm box', fakeAsync(() => {
    const deOpenDialog = fixture.debugElement.query(By.css('#openDialog'));
    const btnOpenDialog: HTMLElement = deOpenDialog.nativeElement;

    btnOpenDialog.click();

    tick(1000);

    spyOn(component.modalRef, 'hide');

    component.closeDialogBox();

    expect(component.modalRef.hide).toHaveBeenCalled();
  }));

  it('should copy the image to clipboard', () => {
    spyOn(component.document, 'execCommand');

    component.divToBeCopiedToClipboard = document.createElement('div');

    component.copyToClipboard();

    expect(component.document.execCommand).toHaveBeenCalledWith('copy');
  });

  it('should download image file when document is NOT general document',  () => {
    spyOn(component, 'downloadImageFile').and.callThrough();
    spyOn(component, 'closeDialogBox');

    const image = {
      FrontImage: { Content: imageService.imageSourceCheck, Width: 1600, Height: 1200 },
      BackImage: { Content: '', Width: 0, Height: 0 }
    };

    const document = new Check((image as Check).FrontImage, (image as Check).BackImage);

    imageService.imageConfig.ImageType = ImageType.Check;
    imageService.setImageConfig(document);

    component.download();

    expect(component.downloadImageFile).toHaveBeenCalled();
    expect(component.closeDialogBox).toHaveBeenCalled();
  });

  it('should download pdf file when document is general document',  () => {
    spyOn(component, 'downloadPdfFile').and.callThrough();
    spyOn(component, 'closeDialogBox');

    const image = {
      DocumentName: 'some name',
      Pages:
      [
          {
            PageNumber: 1,
            FrontImage: { Content: imageService.imageSourceGeneralDoc, Width: 1600, Height: 1200 },
            BackImage: { Content: '', Width: 0, Height: 0 }
          },
          {
            PageNumber: 2,
            FrontImage: { Content: imageService.imageSourceGeneralDoc, Width: 1600, Height: 1200 },
            BackImage: { Content: '', Width: 0, Height: 0 }
          }
      ]
    };

    const document = new GeneralDocument((image as GeneralDocument).DocumentName, (image as GeneralDocument).Pages);

    imageService.imageConfig.ImageType = ImageType.GeneralDocument;
    imageService.setImageConfig(document);

    component.download();

    expect(component.downloadPdfFile).toHaveBeenCalled();
    expect(component.closeDialogBox).toHaveBeenCalled();
  });

  it('should enter fullscreen mode when fullscreen is disabled', () => {
    activatedRoute.testQueryParamMap = { fullscreen: null };
    activatedRoute.testParamMap = { irn: 'ABCDEFGHIJKLMN' };
    activatedRoute.testQueryParamMap = { editable: 'true' };
    activatedRoute.testParamMap = { host: TargetHost.WebClient };

    fixture.detectChanges();

    spyOn(component, 'enterFullscreen').and.callThrough();

    const deEnterFullscreen = fixture.debugElement.query(By.css('#enterFullscreen'));
    const btnEnterFullscreen: HTMLElement = deEnterFullscreen.nativeElement;

    btnEnterFullscreen.click();

    expect(component.enterFullscreen).toHaveBeenCalled();
  });

  it('should enter fullscreen mode when fullscreen is disabled AND doc name is not editable', () => {
    activatedRoute.testQueryParamMap = { fullscreen: null };

    fixture.detectChanges();

    spyOn(component, 'enterFullscreen').and.callThrough();

    const deEnterFullscreen = fixture.debugElement.query(By.css('#enterFullscreen'));
    const btnEnterFullscreen: HTMLElement = deEnterFullscreen.nativeElement;

    btnEnterFullscreen.click();

    expect(component.enterFullscreen).toHaveBeenCalled();
  });

  it('should enter fullscreen mode when fullscreen is disabled AND doc name is editable',
    fakeAsync(inject([TokenService], (tokenService: TokenService) => {

    activatedRoute.testParamMap = { seqNumber : 1 };
    activatedRoute.testQueryParamMap = { fullscreen: null };

    component.isDocumentNameEditable = true;
    component.isEditable = true;
    component.moduleName = 'scan';
    component.host = TargetHost.WebClient;
    component.irn = '123456';

    imageService.imageConfig.ImageFace = ImageFace.Front;
    imageService.imageConfig.ImagePage = 1;

    const basePath = 'viewimage/home';
    const seqNumber = 1;

    const params = `${component.host}/null/${component.irn}/${seqNumber}`;
    let queryParams = `faceImage=${imageService.imageConfig.ImageFace }&fullscreen=true&pageNumber=${imageService.imageConfig.ImagePage}`;
    queryParams += `&editable=${component.isEditable}&source=${component.moduleName}`;

    fixture.detectChanges();

    spyOn(windowService, 'openFullScreen').and.callThrough();

    tokenService.notifyWhenTokenIsValid();

    tick(1000);

    expect(windowService.openFullScreen).toHaveBeenCalledWith(`${basePath}/${params}?${queryParams}`);

    discardPeriodicTasks();
  })));

  it('should enter fullscreen mode when fullscreen is disabled AND default image is loaded',
    fakeAsync(inject([TokenService], (tokenService: TokenService) => {

    activatedRoute.testQueryParamMap = { fullscreen: null };
    imageService.displayDefaultImage = true;

    fixture.detectChanges();

    spyOn(windowService, 'openFullScreen');

    tokenService.notifyWhenTokenIsValid();

    tick(1000);

    expect(windowService.openFullScreen).toHaveBeenCalledWith('viewimage/home?fullscreen=true&default=true&scan=false');

    discardPeriodicTasks();
  })));

  it('should enter fullscreen mode when fullscreen is disabled AND scanned image is loaded',
    fakeAsync(inject([TokenService], (tokenService: TokenService) => {

    activatedRoute.testQueryParamMap = { fullscreen: null };

    imageService.displayDefaultImage = true;
    imageService.displayScannedImage = true;

    fixture.detectChanges();

    spyOn(windowService, 'openFullScreen');

    tokenService.notifyWhenTokenIsValid();

    tick(5000);

    expect(windowService.openFullScreen).toHaveBeenCalledWith('viewimage/home?fullscreen=true&default=true&scan=true');

    discardPeriodicTasks();
  })));

  it('should exit fullscreen mode through EXIT FULLSCREEN button', () => {
    component.isFullscreen = true;

    imageService.imageConfig.ImageSource = 'ABCDE';

    fixture.detectChanges();

    spyOn(component, 'exitFullscreen').and.callThrough();
    spyOn(windowService, 'closeWindow');

    const deExitFullscreen = fixture.debugElement.query(By.css('#exitFullscreen'));
    const btnExitFullscreen: HTMLElement = deExitFullscreen.nativeElement;

    btnExitFullscreen.click();

    expect(component.exitFullscreen).toHaveBeenCalled();
    expect(windowService.closeWindow).toHaveBeenCalled();
  });

  it('should flip to back image', () => {
    spyOn(component, 'resizeImage').and.callThrough();

    component.printDiv = document.createElement('div');

    imageService.imageConfig.ImageFace = ImageFace.Front;

    component.flipImage();

    expect(component.printDiv.innerHTML).toContain('img');
    expect(imageService.imageConfig.ImageFace).toBe(ImageFace.Back);
    expect(component.resizeImage).toHaveBeenCalled();
  });

  it('should flip to front image', () => {
    spyOn(component, 'resizeImage').and.callThrough();

    component.printDiv = document.createElement('div');

    imageService.imageConfig.ImageFace = ImageFace.Back;

    component.flipImage();

    expect(component.printDiv.innerHTML).toContain('img');
    expect(imageService.imageConfig.ImageFace).toBe(ImageFace.Front);
    expect(component.resizeImage).toHaveBeenCalled();
  });

  it('should invert image color', () => {
    imageService.imageConfig.ImageSource = imageService.imageBase64Prefix + imageService.imageSourceCheck;

    const originalColor = imageService.imageConfig.ImageSource;

    component.invertColour();

    expect(originalColor).not.toBe(imageService.imageConfig.ImageSource);
  });

  it('should load general document to be printed', fakeAsync(() => {

    const image = {
      DocumentName: 'some name',
      Pages:
      [
          {
            PageNumber: 1,
            FrontImage: { Content: imageService.imageSourceGeneralDoc, Width: 1600, Height: 1200 },
            BackImage: { Content: '', Width: 0, Height: 0 }
          },
          {
            PageNumber: 2,
            FrontImage: { Content: imageService.imageSourceGeneralDoc, Width: 1600, Height: 1200 },
            BackImage: { Content: '', Width: 0, Height: 0 }
          }
      ]
    };

    const imageDocument = new GeneralDocument((image as GeneralDocument).DocumentName, (image as GeneralDocument).Pages);

    imageService.imageConfig.ImageType = ImageType.GeneralDocument;
    imageService.setImageConfig(imageDocument);

    const printDiv: HTMLDivElement = document.createElement('div');

    component.printDiv = printDiv;

    imageService.notifyChildrenWhenImageIsReady();

    tick(1000);

    expect(imageService.isImageReady).toBeTruthy();
  }));

});
