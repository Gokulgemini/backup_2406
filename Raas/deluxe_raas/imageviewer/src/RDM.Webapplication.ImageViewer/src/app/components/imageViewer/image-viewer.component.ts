import { Component, ViewChild, ElementRef, OnInit, isDevMode, OnDestroy } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { coerceNumberProperty } from '@angular/cdk/coercion';
import { HttpResponse } from '@angular/common/http';

import { timer, Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { ImageService } from '../../services/image.service';
import { WindowService } from '../../services/window.service';
import { ActivityService } from '../../services/activity.service';
import { TokenService } from '../../services/token.service';

import { ImageFace } from '../../constants/imageFace';
import { ImageType } from '../../constants/imageType';
import { ImageFit } from '../../constants/imageFit';
import { ImageDefault } from '../../constants/imageDefault';

import { GeneralDocument } from './../../models/generalDocument';
import { Remittance } from '../../models/remittance';
import { Check } from './../../models/check';

import { rotate } from '../../animations/rotate.animation';

@Component({
    selector: 'image-viewer',
    templateUrl: 'image-viewer.component.html',
    styleUrls: ['image-viewer.component.css'],
    animations: [rotate]
})
export class ImageViewerComponent implements OnInit, OnDestroy {

    @ViewChild('checkImage') checkImage: ElementRef<HTMLImageElement>;
    @ViewChild('clipboardDiv', { static: true }) clipboardDiv: ElementRef;
    @ViewChild('printDiv', { static: true }) printDiv: ElementRef;

    private subscriptionLoadImageStatus: Subscription;
    private subscriptionPageChanged: Subscription;
    private subscriptionImageShapeChanged: Subscription;

    imageDocument: any;

    isLoading = false;
    wasDragged = false;

    sliderValue = 1;
    state = 'default';

    mousePosX;
    mousePosY;
    img_posX;
    img_posY;

    imgWidth_InitialSize = null;
    imgWidth_Before_Zoom;
    imgHeight_Before_Zoom;
    imgWidth_After_Zoom;
    imgHeight_After_Zoom;

    imgWidthProportionsToVerticalAxis = null;
    imgHeightProportionsToHorizontalAxis = null;

    constructor(private activatedRoute: ActivatedRoute,
                private windowService: WindowService,
                private activityService: ActivityService,
                private tokenService: TokenService,
                public imageService: ImageService) { }

    get isFullscreen(): boolean {
        return this.activatedRoute.snapshot.queryParamMap.get('fullscreen') != null;
    }

    ngOnInit() {
        this.loadDocument();
        this.resetPos();
        this.subscribeObservables();
    }

    ngOnDestroy() {
      this.subscriptionLoadImageStatus.unsubscribe();
      this.subscriptionPageChanged.unsubscribe();
      this.subscriptionImageShapeChanged.unsubscribe();
  }

    onMove(event) {
        this.imageService.imageConfig.ImagePosition.x = event.x;
        this.imageService.imageConfig.ImagePosition.y = event.y;
    }

    onStopped() {
        this.resetPos();
        this.wasDragged = true;
        this.imgWidthProportionsToVerticalAxis = null;
    }

    scrollZoom(inOut) {
        this.wasDragged = true;

        this.getDynamicValues(this.sliderValue);

        if (inOut === 'out') {
            this.sliderValue -= 0.1;
        } else {
            this.sliderValue += 0.1;
        }

        if (this.sliderValue > 5) {
            this.sliderValue = 5;
        } else if (this.sliderValue < 0.1) {
            this.sliderValue = 0.1;
        }

        this.getMouseImgRatios();

        this.keepImgFocusPoint();

        this.imageService.imageConfig.ImageFit = '';
    }

    getMouseCoords(event: MouseEvent) {
        this.mousePosX = event.clientX;
        this.mousePosY = event.clientY;
    }

    sliderZoomE(event : any) {
          return this.sliderZoom(event.target.value);
    }

    sliderZoom(sliderV) {
        this.getDynamicValues(sliderV);

        if (this.wasDragged) {

            if (this.imgWidthProportionsToVerticalAxis == null) {
                this.getZoombarImgRatios();
            }

            this.keepImgFocusPoint();

        } else {
            this.keepImgFocusPointBottomRight();
        }

        this.imageService.imageConfig.ImageFit = '';
    }

    private subscribeObservables() {
        this.subscriptionLoadImageStatus = this.imageService.notifyLoadImage$
            .subscribe( isLoading => this.isLoading = isLoading );

        this.subscriptionPageChanged = this.imageService.notifyPageChanged$
            .subscribe( () => this.imageService.setImageConfig(this.imageDocument));

        this.subscriptionImageShapeChanged = this.imageService.notifyImageShapeChanged$
            .subscribe( () => this.ImageShapeChanged());
    }

    private ImageShapeChanged() {
        this.imgWidth_InitialSize = null;
        this.sliderValue = 1;
        this.onStopped();
    }

    private loadDocument() {
        this.isLoading = true;

        if (this.imageService.isDefaultImageRequest) {
            this.loadDocumentMemory();
        } else {
            this.loadDocumentFromDB();
        }
    }

    private loadDocumentMemory() {
        if (this.imageService.isScannedImageRequest) {
            this.loadScannedDocument();
        } else {
            this.loadDefaultDocument();
        }
    }

    private loadDefaultDocument() {
        timer(1000)
        .subscribe(() => {

            this.setDefaultParameters(this.activatedRoute.snapshot.queryParamMap);

            this.initiateDocument(ImageDefault);

            this.isLoading = false;
        });
    }

    private loadScannedDocument() {

        if (isDevMode()) {
            this.imageService.scannedImageUrl = `${this.windowService.nativeWindow.location.origin}/assets/images/no_image_available.png`;
        }

        this.imageService.getScannedImage()
            .then( scannedImage => {
                this.setDefaultParameters(this.activatedRoute.snapshot.queryParamMap);

                this.initiateDocument(scannedImage);

                this.isLoading = false;
            });
    }

    private loadDocumentFromDB() {
        this.activatedRoute.queryParamMap
        .pipe(switchMap(query => {

            const params = this.handleParameters(query);

            return this.imageService.getImages(params.host, params.type, params.irn, params.seqNumber);
        }))
        .subscribe(
            response => {
                this.handleToken(response);

                this.initiateDocument(response.body);

                this.logActivity();

                this.isLoading = false;
            }
        );
    }

    private setDefaultParameters(query: ParamMap) {
        this.imageService.imageConfig.ImageFace = query.get('faceImage') || ImageFace.Front;
        this.imageService.imageConfig.ImagePage = +query.get('pageNumber') || 1;
        this.imageService.imageConfig.ImageType = ImageDefault.Type;
    }

    private handleParameters(query: ParamMap): { host: string, type: string, irn: string, seqNumber: string} {
        this.imageService.imageConfig.ImageFace = query.get('faceImage') || ImageFace.Front;
        this.imageService.imageConfig.ImagePage = +query.get('pageNumber') || 1;

        const irn = this.activatedRoute.snapshot.paramMap.get('irn');
        const host = this.activatedRoute.snapshot.paramMap.get('host');
        const seqNumber = this.activatedRoute.snapshot.paramMap.get('seqNumber');

        this.imageService.imageConfig.ImageType = this.activatedRoute.snapshot.paramMap.get('type');

        const params = {
            host,
            irn,
            type: this.imageService.imageConfig.ImageType,
            seqNumber
        };

        return params;
    }

    private handleToken(resp: HttpResponse<any>) {
        const su_token = resp.headers.get('SUToken');
        this.tokenService.storeSUToken(su_token);
    }

    private initiateDocument(image: any) {

        switch (this.imageService.imageConfig.ImageType) {

            case ImageType.GeneralDocument:
                this.imageDocument = new GeneralDocument(image.DocumentName, image.Pages);
                break;

            case ImageType.Remittance:
                this.imageDocument = new Remittance(image.IsVirtual, image.FrontImage, image.BackImage);
                break;

            default:
                this.imageDocument = new Check(image.FrontImage, image.BackImage);
                break;

        }

        this.setImagePropertiesOnload();
    }

    private setImagePropertiesOnload() {

        this.imageService.setImageConfig(this.imageDocument);

        if (this.imageService.isGeneralDcoument) {
            this.imageService.imageConfig.ImageWidth =
                this.imageDocument.Pages[this.imageService.imageConfig.ImagePage - 1].FrontImage.Width;
            this.imageService.imageConfig.ImageTotalPage = this.imageDocument.Pages.length;
        } else {
            this.imageService.imageConfig.ImageWidth = this.imageDocument.FrontImage.Width;
        }

        this.imageService.imageConfig.ImageName = this.imageDocument.DocumentName;

        this.imageService.resizeImage(ImageFit.OnLoad);

        this.imageService.notifyChildrenWhenImageIsReady();

        this.resetPos();
    }

    private logActivity() {
        if (this.isFullscreen || (this.imageService.imageConfig.ImageType === ImageType.Remittance && this.imageDocument.IsVirtual) ) {
                return;
        }

        this.activityService.logActivity();
    }

    private getMouseImgRatios() {
        this.imgWidthProportionsToVerticalAxis = ((this.mousePosX - this.img_posX) / (this.imgWidth_Before_Zoom));
        this.imgHeightProportionsToHorizontalAxis = ((this.mousePosY - this.img_posY) / this.imgHeight_Before_Zoom);
    }

    private getDynamicValues(sliderV) {
        if (this.imgWidth_InitialSize === null) {
            this.imgWidth_InitialSize = this.imageService.imageConfig.ImageWidth;
            this.imageService.imageConfig.ImageHeight = this.checkImage.nativeElement.height;
        }

        this.imgWidth_After_Zoom = this.imgWidth_InitialSize * coerceNumberProperty(sliderV);
        this.imgHeight_After_Zoom = this.imageService.imageConfig.ImageHeight * coerceNumberProperty(sliderV);

        if (this.wasDragged) {
            this.imgWidth_Before_Zoom = this.imageService.imageConfig.ImageWidth;
            this.imgHeight_Before_Zoom = this.checkImage.nativeElement.height;
        }

        this.imageService.imageConfig.ImageWidth = this.imgWidth_After_Zoom;
    }

    private getZoombarImgRatios() {
        this.imgWidthProportionsToVerticalAxis =
            (((this.windowService.nativeWindow.innerWidth / 2) - this.img_posX) / (this.imgWidth_After_Zoom));
        this.imgHeightProportionsToHorizontalAxis =
            (((this.windowService.nativeWindow.innerHeight / 2) - this.img_posY) / (this.imgHeight_After_Zoom));
    }

    private keepImgFocusPoint() {
        this.imageService.imageConfig.ImagePosition = {
            x: ((this.imgWidthProportionsToVerticalAxis * (this.imgWidth_Before_Zoom - this.imgWidth_After_Zoom)) + this.img_posX),
            y: ((this.imgHeightProportionsToHorizontalAxis * (this.imgHeight_Before_Zoom - this.imgHeight_After_Zoom)) + this.img_posY)
        };
        this.resetPos();
    }

    private keepImgFocusPointBottomRight() {
        this.imageService.imageConfig.ImagePosition = {
            x: (this.imgWidth_InitialSize - this.imgWidth_After_Zoom + this.img_posX),
            y: (this.imageService.imageConfig.ImageHeight - this.imgHeight_After_Zoom + this.img_posY)
        };
    }

    private resetPos() {
        this.img_posX = this.imageService.imageConfig.ImagePosition.x;
        this.img_posY = this.imageService.imageConfig.ImagePosition.y;
    }
}
