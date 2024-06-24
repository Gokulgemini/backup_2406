import { TestBed, inject } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController  } from '@angular/common/http/testing';
import { ActivatedRoute } from '@angular/router';

import { ConfigService } from './config.service';
import { WindowService } from './window.service';
import { ImageService } from './image.service';

import { ImageFit } from '../constants/imageFit';
import { ImageType } from '../constants/imageType';
import { TargetHost } from '../constants/targetHost';

import { ImageFace } from '../constants/imageFace';
import { Check } from '../models/check';

import { Remittance } from '../models/remittance';
import { GeneralDocument } from '../models/generalDocument';
import { GeneralDocumentPage } from '../models/generalDocumentPage';
import { BackImage } from '../models/backImage';
import { FrontImage } from '../models/frontImage';

import { ActivatedRouteStub } from '../stubs/activateRoute.stub';

describe('ImageService', () => {

    let activatedRoute: ActivatedRouteStub;

    beforeEach(() => {

        activatedRoute = new ActivatedRouteStub();

        TestBed.configureTestingModule({
            imports: [ HttpClientTestingModule ],
            declarations: [],
            providers: [
                WindowService,
                ConfigService,
                ImageService,
                { provide: ActivatedRoute, useValue: activatedRoute }
            ]
        }).compileComponents();
    });

    it('should create image service', inject([ImageService], (service: ImageService) => {
        expect(service).toBeTruthy();
    }));

    it('should maximize image width', inject([ImageService], (service: ImageService) => {
        service.resizeImage(ImageFit.BestWidth);

        expect(service.imageConfig.ImageWidth).toEqual(window.innerWidth);
    }));

    it('should maximize image height', inject([ImageService], (service: ImageService) => {

        service.imageConfig.ImageFace = ImageFace.Front;
        service.imageDimensionRatio.imageDimensionRatio_front = 1.5;

        service.resizeImage(ImageFit.BestHeight);

        const correctHeight = window.innerHeight - 30;

        expect(service.imageConfig.ImageWidth).toEqual( 1.5 * correctHeight);
    }));


    it('should fit image width', inject([ImageService], (service: ImageService) => {

        const width = window.innerWidth;

        service.imageConfig.ImageFace = ImageFace.Front;
        service.imageDimensionRatio.imageDimensionRatio_front = 3;

        service.resizeImage(ImageFit.Fit);

        expect(service.imageConfig.ImageWidth).toEqual(width);
    }));

    it('should fit front image height', inject([ImageService], (service: ImageService) => {

        service.imageConfig.ImageFace = ImageFace.Front;
        service.imageConfig.ImageDimensionRatio = 0;
        service.imageConfig.ImageWidth = 2000;
        service.imageConfig.ImageHeight  = 500;
        service.imageConfig.ImagePosition  = {
            x: 0,
            y: 0
        };

        const height = 0;

        service.resizeImage(ImageFit.Fit);

        expect(service.imageConfig.ImageWidth).toEqual(height);
    }));

    it('should fit back image height', inject([ImageService], (service: ImageService) => {

        service.imageConfig.ImageFace = ImageFace.Back;
        service.imageConfig.ImageDimensionRatio = 0;
        service.imageConfig.ImageWidth = 2000;
        service.imageConfig.ImageHeight  = 500;
        service.imageConfig.ImagePosition  = {
            x: 0,
            y: 0
        };

        const height = 0;

        service.resizeImage(ImageFit.Fit);

        expect(service.imageConfig.ImageWidth).toEqual(height);
    }));

    it('should fit image during the load event', inject([ImageService], (service: ImageService) => {

        service.imageConfig.ImageDimensionRatio = 0;
        service.imageConfig.ImageWidth = 2000;
        service.imageConfig.ImageHeight  = 500;
        service.imageConfig.ImagePosition  = {
            x: 0,
            y: 0
        };

        const height = 0;

        service.resizeImage(ImageFit.OnLoad);

        expect(service.imageConfig.ImageWidth).toEqual(height);
    }));

    it('should keep image as is', inject([ImageService], (service: ImageService) => {

        const imageDimensionRatio = 0;
        const imageWidth = 2000;
        const imageHeight  = 500;

        service.imageConfig.ImageDimensionRatio = imageDimensionRatio;
        service.imageConfig.ImageWidth = imageWidth;
        service.imageConfig.ImageHeight  = imageHeight;

        service.resizeImage('');

        expect(service.imageConfig.ImageHeight).toEqual(imageHeight);
        expect(service.imageConfig.ImageWidth).toEqual(imageWidth);
        expect(service.imageConfig.ImageDimensionRatio).toEqual(imageDimensionRatio);
    }));

    it('should get check image from ITMS db', inject([ImageService, ConfigService, HttpTestingController],
                                        (service: ImageService, config: ConfigService, http: HttpTestingController) => {

        const host = TargetHost.ITMS;
        const irn = 'ABCDEF';
        const seqNum = '123';

        service.getImages(host, ImageType.Check, irn, seqNum);

        const req = http.expectOne(`${config.baseUrl}/itms/check/${irn}?targetHost=${host}&seqNum=${seqNum}`);
        expect(req.request.method).toBe('GET');
    }));

    it('should get remittance image from ITMS db', inject([ImageService, ConfigService, HttpTestingController], (
                                    service: ImageService, config: ConfigService, http: HttpTestingController) => {

        const host = TargetHost.ITMS;
        const irn = 'ABCDEF';
        const seqNum = '123';

        service.getImages(host, ImageType.Remittance, irn, seqNum);

        const req = http.expectOne(`${config.baseUrl}/itms/remittance/${irn}?targetHost=${host}&seqNum=${seqNum}`);
        expect(req.request.method).toBe('GET');
    }));

    it('should get general document image from ITMS db', inject([ImageService, ConfigService, HttpTestingController],
                                            (service: ImageService, config: ConfigService, http: HttpTestingController) => {

        const host = TargetHost.ITMS;
        const irn = 'ABCDEF';
        const seqNum = '123';

        service.getImages(host, ImageType.GeneralDocument, irn, seqNum);

        const req = http.expectOne(`${config.baseUrl}/itms/generaldocument/${irn}?targetHost=${host}&seqNum=${seqNum}`);
        expect(req.request.method).toBe('GET');
    }));

    it('should get check image from WebClient db', inject([ImageService, ConfigService, HttpTestingController],
                                              (service: ImageService, config: ConfigService, http: HttpTestingController) => {

        const host = TargetHost.WebClient;
        const irn = 'ABCDEF';

        service.getImages(host, ImageType.Check, irn);

        const req = http.expectOne(`${config.baseUrl}/itms/check/${irn}?targetHost=${host}`);
        expect(req.request.method).toBe('GET');
    }));

    it('should get remittance image from WebClient db', inject([ImageService, ConfigService, HttpTestingController], (
                                                   service: ImageService, config: ConfigService, http: HttpTestingController) => {

        const host = TargetHost.WebClient;
        const irn = 'ABCDEF';

        service.getImages(host, ImageType.Remittance, irn);

        const req = http.expectOne(`${config.baseUrl}/itms/remittance/${irn}?targetHost=${host}`);
        expect(req.request.method).toBe('GET');
    }));

    it('should get general document image from WebClient db', inject([ImageService, ConfigService, HttpTestingController],
                                                        (service: ImageService, config: ConfigService, http: HttpTestingController) => {

        const host = TargetHost.WebClient;
        const irn = 'ABCDEF';

        service.getImages(host, ImageType.GeneralDocument, irn);

        const req = http.expectOne(`${config.baseUrl}/itms/generaldocument/${irn}?targetHost=${host}`);
        expect(req.request.method).toBe('GET');
    }));

    it('should get NULL when the document type is not recognized', inject([ImageService], (service: ImageService) => {

        const irn = 'ABCDEF';
        const host = 'Itms';
	
        let result = service.getImages(host, '', irn, '');

        expect(result).toBeNull();
    }));

    it('should return front image as imageconfig source for checks', inject([ImageService], (service: ImageService) => {

        const imageDocument = new Check(new FrontImage('XYZ', 10, 10), new BackImage('ABC', 10, 10));

        const frontImageContent = imageDocument.FrontImage.Content;

        service.imageConfig.ImageType = ImageType.Check;
        service.imageConfig.ImagePage = 1;

        service.setImageConfig(imageDocument);

        expect(service.imageConfig.ImageSource).toContain(frontImageContent);
    }));

    it('should return back image as imageconfig source for checks', inject([ImageService], (service: ImageService) => {

        const imageDocument = new Check(new FrontImage('XYZ', 10, 10), new BackImage('ABC', 10, 10));

        const backImageContent = imageDocument.BackImage.Content;

        service.imageConfig.ImageType = ImageType.Check;
        service.imageConfig.ImageFace = ImageFace.Back;
        service.imageConfig.ImagePage = 1;

        service.setImageConfig(imageDocument);

        expect(service.imageConfig.ImageSource).toContain(backImageContent);
    }));

    it('should return front image as imageconfig source for remittances', inject([ImageService], (service: ImageService) => {

        const imageDocument = new Remittance(false, new FrontImage('XYZ', 10, 10), new BackImage('ABC', 10, 10));

        const frontImageContent = imageDocument.FrontImage.Content;

        service.imageConfig.ImageType = ImageType.Check;
        service.imageConfig.ImagePage = 1;

        service.setImageConfig(imageDocument);

        expect(service.imageConfig.ImageSource).toContain(frontImageContent);
    }));

    it('should return back image as imageconfig source for remittances', inject([ImageService], (service: ImageService) => {

        const imageDocument = new Remittance(false, new FrontImage('XYZ', 10, 10), new BackImage('ABC', 10, 10));

        const backImageContent = imageDocument.BackImage.Content;

        service.imageConfig.ImageType = ImageType.Check;
        service.imageConfig.ImageFace = ImageFace.Back;
        service.imageConfig.ImagePage = 1;

        service.setImageConfig(imageDocument);

        expect(service.imageConfig.ImageSource).toContain(backImageContent);
    }));

    it('should return front image as imageconfig source for general documents', inject([ImageService], (service: ImageService) => {

        const imageDocument = new GeneralDocument(
            '', [new GeneralDocumentPage(1, new FrontImage('XYZ', 10, 10), new BackImage('', 10, 10))]
        );

        const frontImageContent = imageDocument.Pages[0].FrontImage.Content;

        service.imageConfig.ImageType = ImageType.GeneralDocument;
        service.imageConfig.ImagePage = 1;

        service.setImageConfig(imageDocument);

        expect(service.imageConfig.ImageSource).toContain(frontImageContent);
    }));

    it('should convert back pages to front pages when document is GeneralDocument type and it contains back pages',
      inject([ImageService], (service: ImageService) => {

        const image = {
            DocumentName: 'some name',
            Pages:
            [
                {
                    PageNumber: 1,
                    FrontImage: { Content: 'ABC', Width: 10, Height: 10 },
                    BackImage: { Content: 'ABC', Width: 10, Height: 10 }
                },
                {
                    PageNumber: 2,
                    FrontImage: { Content: 'ABC', Width: 10, Height: 10 },
                    BackImage: { Content: 'ABC', Width: 10, Height: 10 }
                },
                {
                    PageNumber: 3,
                    FrontImage: { Content: 'ABC', Width: 10, Height: 10 },
                    BackImage: { Content: 'ABC', Width: 10, Height: 10 }
                }
            ]
        };

        const pageCount = image.Pages.length;

        const document = new GeneralDocument((image as GeneralDocument).DocumentName, (image as GeneralDocument).Pages);

        const pageCountAfterConvertion = document.Pages.length;

        service.imageConfig.ImageType = ImageType.GeneralDocument;
        service.setImageConfig(document);

        expect(pageCount).not.toBe(pageCountAfterConvertion);
        expect(service.documentPages.length).toBe(6);
    }));

   it('should keep the document as is when document is GeneralDocument type and it DOES NOT contain back pages',
     inject([ImageService], (service: ImageService) => {

        const image = {
            DocumentName: 'some name',
            Pages:
            [
                { PageNumber: 1, FrontImage: { Content: 'ABC', Width: 10, Height: 10 }, BackImage: { Content: '', Width: 0, Height: 0 } },
                { PageNumber: 2, FrontImage: { Content: 'ABC', Width: 10, Height: 10 }, BackImage: { Content: '', Width: 0, Height: 0 } },
                { PageNumber: 3, FrontImage: { Content: 'ABC', Width: 10, Height: 10 }, BackImage: { Content: '', Width: 0, Height: 0 } }
            ]
        };

        const pageCount = image.Pages.length;

        const document = new GeneralDocument((image as GeneralDocument).DocumentName, (image as GeneralDocument).Pages);

        const pageCountAfterConvertion = document.Pages.length;

        service.imageConfig.ImageType = ImageType.GeneralDocument;
        service.setImageConfig(document);

        expect(pageCount).toBe(pageCountAfterConvertion);
        expect(service.documentPages.length).toBe(3);
    }));

    it('should get imageBase64Prefix value', inject([ImageService], (service: ImageService) => {
        expect(service.imageBase64Prefix).toBe('data:image/png;base64,');
    }));

    it('should return FALSE when checking whether document is general document or not when imageConfig is null',
    inject([ImageService], (service: ImageService) => {
        service.imageConfig = null;

        expect(service.isGeneralDcoument).toBeFalsy();
    }));

    it('should display image when it is fully loaded', inject([ImageService], (service: ImageService) => {
        const image = {
            DocumentName: 'some name',
            Pages:
            [
                {
                    PageNumber: 1,
                    FrontImage: { Content: 'ABC', Width: 10, Height: 10 },
                    BackImage: { Content: '', Width: 0, Height: 0 }
                }
            ]
        };

        const document = new GeneralDocument((image as GeneralDocument).DocumentName, (image as GeneralDocument).Pages);

        service.imageConfig.ImageType = ImageType.GeneralDocument;
        service.setImageConfig(document);

        expect(service.isImageReady).toBeTruthy();
    }));

    it('should NOT display image when imageConfig is null', inject([ImageService], (service: ImageService) => {
        service.imageConfig = null;

        expect(service.isImageReady).toBeFalsy();
    }));

    it('should return TRUE (HAS BACK IMAGE) when it has back image', inject([ImageService], (service: ImageService) => {
        const document = new Check(new FrontImage('XYZ', 10, 10), new BackImage('ABC', 10, 10));

        service.imageConfig.ImageType = ImageType.Check;
        service.setImageConfig(document);

        expect(service.hasBackImage).toBeTruthy();
    }));

    it('should return FALSE (NO BACK IMAGE) when document is NOT general document BUT it has NO back image',
      inject([ImageService], (service: ImageService) => {

        const image = new Remittance(false, new FrontImage('XYZ', 10, 10), new BackImage('', 0, 0));

        const document = new Remittance((image as Remittance).IsVirtual,
                                        (image as Remittance).FrontImage,
                                        (image as Remittance).BackImage);

        service.imageConfig.ImageType = ImageType.Remittance;
        service.setImageConfig(document);

        expect(service.hasBackImage).toBeFalsy();
    }));

    it('should return FALSE (NO BACK IMAGE) when document is general document', inject([ImageService], (service: ImageService) => {
        const image = {
            DocumentName: 'some name',
            Pages:
            [
                {
                    PageNumber: 1,
                    FrontImage: { Content: 'ABC', Width: 10, Height: 10 },
                    BackImage: { Content: '', Width: 0, Height: 0 }
                }
            ]
        };

        const document = new GeneralDocument((image as GeneralDocument).DocumentName, (image as GeneralDocument).Pages);

        service.imageConfig.ImageType = ImageType.GeneralDocument;
        service.setImageConfig(document);

        expect(service.hasBackImage).toBeFalsy();
    }));

    it('should return FALSE (NO BACK IMAGE) when imageConfig is null', inject([ImageService], (service: ImageService) => {
        service.imageConfig = null;

        expect(service.hasBackImage).toBeFalsy();
    }));

    it('should notify subscribers when image is ready', inject([ImageService], (service: ImageService) => {
        const document = new Check(new FrontImage('XYZ', 10, 10), new BackImage('ABC', 10, 10));

        service.imageConfig.ImageType = ImageType.Check;
        service.setImageConfig(document);

        service.notifyChildrenWhenImageIsReady();

        service.notifyImageReady$.subscribe( isImageReady => {
            expect(isImageReady).toBeTruthy();
        });
    }));

    it('should notify subscribers when document name has change', inject([ImageService], (service: ImageService) => {
        const document = new GeneralDocument(
            'Doc Name', [new GeneralDocumentPage(1, new FrontImage('XYZ', 10, 10), new BackImage('', 10, 10))]
        );

        service.imageConfig.ImageType = ImageType.GeneralDocument;
        service.setImageConfig(document);

        const docNameChanged = 'Edit Doc Name';

        service.notifyChildrenWhenDocNameChanged(docNameChanged);

        service.notifyDocNameChange$.subscribe( docName => {
            expect(docName).toBe(docNameChanged);
        });
    }));

    it('should update document name', inject([ImageService, ConfigService, HttpTestingController],
                                            (service: ImageService, config: ConfigService, http: HttpTestingController) => {

        const irn = 'ABCDEF';
        const docName = 'Doc Name';
        const moduleName = 'Module Name';

        service.updateDocumentName(irn, docName, moduleName).subscribe();

        const req = http.expectOne(`${config.baseUrl}/itms/generaldocument/${irn}`);
        expect(req.request.method).toBe('PUT');
    }));

    it('should return TRUE when a default image must be displayed', inject([ImageService], (service: ImageService) => {
        activatedRoute.testQueryParamMap = { default: 'true' };

        expect(service.isDefaultImageRequest).toBeTruthy();
    }));

    it('should return FALSE when a default image must NOT be displayed', inject([ImageService], (service: ImageService) => {
        activatedRoute.testQueryParamMap = { default: 'false' };

        expect(service.isDefaultImageRequest).toBeFalsy();
    }));

    it('should return FALSE when an actual image from db must be displayed', inject([ImageService], (service: ImageService) => {
        activatedRoute.testQueryParamMap = { default: null };

        expect(service.isDefaultImageRequest).toBeFalsy();
    }));

    it('should return TRUE when a scanned image must be displayed', inject([ImageService], (service: ImageService) => {
        activatedRoute.testQueryParamMap = { default: 'true' };
        activatedRoute.testQueryParamMap = { scan: 'true' };

        expect(service.isScannedImageRequest).toBeTruthy();
    }));

    it('should return FALSE when a scanned image must NOT be displayed', inject([ImageService], (service: ImageService) => {
        activatedRoute.testQueryParamMap = { default: 'true' };
        activatedRoute.testQueryParamMap = { scan: 'false' };

        expect(service.isScannedImageRequest).toBeFalsy();
    }));

    it('should return FALSE when the scan param is NOT provided', inject([ImageService], (service: ImageService) => {
        activatedRoute.testQueryParamMap = { default: 'true' };

        expect(service.isScannedImageRequest).toBeFalsy();
    }));

    it('should return image JSON ', inject([ImageService], (service: ImageService) => {
        activatedRoute.testQueryParamMap = { default: 'true' };
        activatedRoute.testQueryParamMap = { scan: 'true' };

        const imageSource = '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';

        service.scannedImageUrl = 'data:image/png;base64,' + imageSource;

        const imageJson = {
                            FrontImage:
                            {
                                Content: imageSource,
                                Width: 1200,
                                Height: 550
                            },
                            BackImage:
                            {
                                Content: '',
                                Width: 0,
                                Height: 0
                            }
                        };

        service.getScannedImage().then((scannedImage: any) => {
            expect(scannedImage.FrontImage.Content).toContain(imageJson.FrontImage.Content.substr(0, 20));
            expect(scannedImage.FrontImage.Height).toBe(imageJson.FrontImage.Height);
            expect(scannedImage.FrontImage.Width).toBe(imageJson.FrontImage.Width);
        });
    }));

    it('should return NULL when isScannedImageRequest property is FALSE  ', inject([ImageService], (service: ImageService) => {
        activatedRoute.testQueryParamMap = { default: 'true' };
        activatedRoute.testQueryParamMap = { scan: 'false' };

        service.getScannedImage().then(scannedImage => {
            expect(scannedImage).toBeNull();
        });
    }));

    it('should notify subscribers when load image status changes', inject([ImageService], (service: ImageService) => {
        service.notifyLoadImageStatus(false);

        service.notifyDocNameChange$.subscribe( isLoading => {
            expect(isLoading).toBeFalsy();
        });
    }));

    it('should notify subscribers when document page index changes', inject([ImageService], (service: ImageService) => {
        spyOn(service, 'resizeImage');

        service.notifyWhenPageChanged();

        service.notifyPageChanged$.subscribe( () => {
            service.resizeImage(ImageFit.OnLoad);
            expect(service.resizeImage).toHaveBeenCalled();
        });
    }));

    it('should notify subscribers when image shape changes', inject([ImageService], (service: ImageService) => {
        spyOn(service, 'resizeImage');

        service.notifyWhenImageShapeChanged();

        service.notifyImageShapeChanged$.subscribe( () => {
            service.resizeImage(ImageFit.OnLoad);
            expect(service.resizeImage).toHaveBeenCalled();
        });
    }));
});
