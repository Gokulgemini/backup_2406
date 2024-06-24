import { Injectable, RendererFactory2, Renderer2 } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';​
import { ActivatedRoute } from '@angular/router';
import { of, Subject, Observable, lastValueFrom } from 'rxjs';

import { ImageConfig } from '../vo/imageConfig';
import { ImageDimentionRatio } from '../vo/imageDimensionRatio';

import { GeneralDocument } from '../models/generalDocument';
import { GeneralDocumentPage } from './../models/generalDocumentPage';
import { Remittance } from '../models/remittance';
import { Check } from '../models/check';

import { ImageFit } from '../constants/imageFit';
import { ImageType } from '../constants/imageType';
import { ImageFace } from '../constants/imageFace';
import { TargetHost } from '../constants/targetHost';

import { ConfigService } from './config.service';
import { WindowService } from './window.service';

import { StringUtils } from '../utils/string.utils';

@Injectable({
    providedIn: 'root'
  })
  
export class ImageService {

    private readonly _imageBase64Prefix = 'data:image/png;base64,';

    private notifyImageReady = new Subject<boolean>();
    private notifyDocNameChange = new Subject<string>();
    private notifyLoadImage = new Subject<boolean>();
    private notifyPageChanged = new Subject<void>();
    private notifyImageShapeChanged = new Subject<void>();

    private baseUrl: string;
    private generalDocumentPages: GeneralDocumentPage[];
    private renderer: Renderer2;

    scannedImageUrl = `${this.windowService.nativeWindow.location.origin}/WebClient2/V20/Common/ImageDisplay.aspx?Test=true`;

    imageDimensionRatio: ImageDimentionRatio;
    endpoint: string;
    imageConfig: ImageConfig;

    notifyImageReady$: Observable<boolean>;
    notifyDocNameChange$: Observable<string>;
    notifyLoadImage$: Observable<boolean>;
    notifyPageChanged$: Observable<any>;
    notifyImageShapeChanged$: Observable<any>;


    constructor(configService: ConfigService,
                rendererFactory: RendererFactory2,
                private http: HttpClient,
                private windowService: WindowService,
                private activatedRoute: ActivatedRoute) {

                this.baseUrl = configService.baseUrl;
                this.imageDimensionRatio = new ImageDimentionRatio();
                this.imageConfig = new ImageConfig;

                this.renderer = rendererFactory.createRenderer(null, null);

                this.notifyImageReady$ = this.notifyImageReady.asObservable();
                this.notifyDocNameChange$ = this.notifyDocNameChange.asObservable();
                this.notifyLoadImage$ = this.notifyLoadImage.asObservable();
                this.notifyPageChanged$ = this.notifyPageChanged.asObservable();
                this.notifyImageShapeChanged$ = this.notifyImageShapeChanged.asObservable();
    }

    get documentPages() {
        return this.generalDocumentPages;
    }

    get imageBase64Prefix() {
        return this._imageBase64Prefix;
    }

    get isImageReady(): boolean {
        if (!this.imageConfig) { return false; }
        return !StringUtils.isNullOrEmpty(this.imageConfig.ImageSource);
    }

    get isGeneralDcoument(): boolean {
        if (!this.imageConfig) { return false; }
        return this.imageConfig.ImageType === ImageType.GeneralDocument;
    }

    get hasBackImage(): boolean {
        if (!this.imageConfig) { return false; }
        return !StringUtils.isNullOrEmpty(this.imageConfig.ImageBackContent);
    }

    get isDefaultImageRequest(): boolean {
        const defaultImage = this.activatedRoute.snapshot.queryParamMap.get('default') || 'false';
        return defaultImage === 'false' ? false : true;
    }

    get isScannedImageRequest(): boolean {
        const scanImage = this.activatedRoute.snapshot.queryParamMap.get('scan') || 'false';
        return scanImage === 'false' ? false : true;
    }

    notifyChildrenWhenDocNameChanged(docName) {
        this.notifyDocNameChange.next(docName);
    }

    notifyChildrenWhenImageIsReady() {
        this.notifyImageReady.next(this.isImageReady);
    }

    notifyLoadImageStatus(isLoading: boolean) {
        this.notifyLoadImage.next(isLoading);
    }

    notifyWhenPageChanged() {
        this.notifyPageChanged.next();
    }

    notifyWhenImageShapeChanged() {
        this.notifyImageShapeChanged.next();
    }

    getImages(host: string, itemType: string, irn: string, seqNum?: string) {

        let params: HttpParams;

        switch (host)
        {
            case TargetHost.ITMS:
                params = new HttpParams().set('targetHost', host).set('seqNum', seqNum as string);
                break;
            case TargetHost.WebClient:
                params = new HttpParams().set('targetHost', host);
                break;
            default:
                return of(null);
        }

        switch (itemType) {
            case ImageType.Check:
                return lastValueFrom(this.http.get<Check>(`${this.baseUrl}/itms/check/${irn}`, { params, observe: 'response' }));

            case ImageType.Remittance:
                return lastValueFrom(this.http.get<Remittance>(`${this.baseUrl}/itms/remittance/${irn}`, { params, observe: 'response' }));

            case ImageType.GeneralDocument:
                return lastValueFrom(this.http.get<GeneralDocument>(`${this.baseUrl}/itms/generaldocument/${irn}`, { params, observe: 'response' }));

            default:
        }
        return null;
    }

    updateDocumentName(irn: string, documentName: string, module: string) {

        return this.http.put(`${this.baseUrl}/itms/generaldocument/${irn}`, { documentName, module });
    }

    resizeImage(shape: string) {
        if (this.imageConfig === null) return;
        if (this.imageConfig.ImageFace === ImageFace.Front) {
            this.imageConfig.ImageDimensionRatio = this.imageDimensionRatio.imageDimensionRatio_front;
        } else {
            this.imageConfig.ImageDimensionRatio = this.imageDimensionRatio.imageDimensionRatio_back;
        }

        this.imageConfig.ImageFit = shape;

        switch (shape) {
            case ImageFit.Fit:
                this.bestFit();
            break;
            case ImageFit.BestHeight:
                this.fullHeight();
            break;
            case ImageFit.BestWidth:
                this.fullWidth();
            break;
            case ImageFit.OnLoad:
                this.bestFit();
            break;
            default:
            break;
         }
    }

    setImageConfig(imageDocument: any) {
        if (!imageDocument) {
            return;
        }

        const index = this.imageConfig.ImagePage - 1;

        if (this.isGeneralDcoument) {
            this.generalDocumentPages = imageDocument.Pages;
            this.setBaseConfig(imageDocument.Pages[index]);
        } else {
            this.setBaseConfig(imageDocument);
        }
    }

    getScannedImage() {
        if (!this.isScannedImageRequest) { return new Promise(resolve => resolve(null)); }

        return new Promise(resolve => {
            const img = new Image();

            img.src = this.scannedImageUrl;

            img.onload = () => resolve(this.convertScannedUrlImageToImageJson(img));
        });
    }

    private convertScannedUrlImageToImageJson(img: HTMLImageElement) {
        const canvas: HTMLCanvasElement  = this.renderer.createElement('canvas');

        canvas.width = img.width;
        canvas.height = img.height;

        const context = canvas.getContext('2d');
        context.drawImage(img, 0, 0);

        let dataURL = canvas.toDataURL();
        dataURL = dataURL.replace(/^data:image\/(png|jpg);base64,/, '');

       return {
                    FrontImage:
                    {
                        Content: dataURL,
                        Width: img.width,
                        Height: img.height
                    },
                    BackImage:
                    {
                        Content: '',
                        Width: 0,
                        Height: 0
                    }
                };
    }

    private setBaseConfig(imageDocument: any) {
    
        let imgSource = '';

        this.imageDimensionRatio.imageDimensionRatio_front = imageDocument.FrontImage.Width / imageDocument.FrontImage.Height;
        this.imageDimensionRatio.imageDimensionRatio_back = imageDocument.BackImage.Width / imageDocument.BackImage.Height;

        if (this.imageConfig.ImageFace === ImageFace.Back) {
            imgSource = imageDocument.BackImage.Content;
        } else {
            imgSource = imageDocument.FrontImage.Content;
        }

        this.imageConfig.ImageBackContent = imageDocument.BackImage.Content;
        this.imageConfig.ImageFrontContent = imageDocument.FrontImage.Content;

        this.imageConfig.ImageSource = this._imageBase64Prefix + imgSource;
    }

    private bestFit() {
        if ((this.windowService.nativeWindow.innerWidth / this.imageConfig.ImageDimensionRatio) >
            (this.windowService.nativeWindow.innerHeight - 30)) {
            this.fullHeight();
        } else {
            this.fullWidth();
        }
    }

    private fullWidth() {
        this.imageConfig.ImageWidth = this.windowService.nativeWindow.innerWidth;
        this.imageConfig.ImagePosition = { x: 0, y: 30 };
    }

    private fullHeight() {
        this.imageConfig.ImageWidth = (this.imageConfig.ImageDimensionRatio * (this.windowService.nativeWindow.innerHeight - 30));

        if (this.imageConfig.ImageFit === ImageFit.OnLoad) {
            this.imageConfig.ImagePosition = {
                x: this.windowService.nativeWindow.innerWidth - this.imageConfig.ImageWidth,
                y: 30
            };
        } else {
            this.imageConfig.ImagePosition = {
                x: (this.windowService.nativeWindow.innerWidth / 2) - (this.imageConfig.ImageWidth / 2),
                y: 30
            };
        }
    }
}
