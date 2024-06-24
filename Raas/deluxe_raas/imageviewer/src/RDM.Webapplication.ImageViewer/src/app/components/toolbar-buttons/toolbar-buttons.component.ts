import { Component, OnInit, Input, Inject, TemplateRef, Renderer2, OnDestroy } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { Subscription, timer  } from 'rxjs';
import { ActivatedRoute } from '@angular/router';

import { BsModalService } from 'ngx-bootstrap/modal';
import { BsModalRef } from 'ngx-bootstrap/modal/bs-modal-ref.service';

import { ImageService } from '../../services/image.service';
import { SessionService } from '../../services/session.service';
import { WindowService } from '../../services/window.service';
import { TokenService } from 'src/app/services/token.service';

import { ImageFace } from '../../constants/imageFace';

import 'to-blob';
import { saveAs } from 'file-saver';
import jsPDF from 'jspdf';

@Component({
  selector: 'toolbar-buttons',
  templateUrl: './toolbar-buttons.component.html',
  styleUrls: ['./toolbar-buttons.component.css']
})
export class ToolbarButtonsComponent implements OnInit, OnDestroy {

  @Input('divToBeCopiedToClipboard') divToBeCopiedToClipboard;
  @Input('divToBePrinted') printDiv;
  @Input('irn') irn: string;
  @Input('host') host: string;
  @Input('moduleName') moduleName: string;
  @Input('isFullscreen') isFullscreen: boolean;
  @Input('isEditable') isEditable: boolean;
  @Input('isDocumentNameEditable') isDocumentNameEditable: boolean;

  modalRef: BsModalRef;
  isFullScreenButtonDisabled  = false;

  private context: CanvasRenderingContext2D;
  private canvas: HTMLCanvasElement;

  private subscriptionImageReady: Subscription;
  private subscriptionTokenStatus: Subscription;

  constructor(private sessionService: SessionService,
              public imageService: ImageService,
              private windowService: WindowService,
              private tokenService: TokenService,
              private modalService: BsModalService,
              private activatedRoute: ActivatedRoute,
              private renderer: Renderer2,
              @Inject(DOCUMENT) public document: any) { }

  get showFlipButton(): boolean {
    return !this.imageService.isGeneralDcoument && this.imageService.hasBackImage;
  }

  get isInternetExplorer() {
    return !(navigator.userAgent.indexOf('Trident') === -1);
  }

  ngOnInit() {
    this.subscriptionImageReady = this.imageService.notifyImageReady$
      .subscribe( result => { if (result) { this.loadImagesToPrint(); } });

    this.subscriptionTokenStatus = this.tokenService.notifyTokenIsValid$
      .subscribe( () => this.handleFullscreenAction() );
  }

  ngOnDestroy() {
    this.subscriptionImageReady.unsubscribe();
    this.subscriptionTokenStatus.unsubscribe();
}

  flipImage() {
    let imgSource = '';

    if (this.imageService.imageConfig.ImageFace === ImageFace.Back) {
        this.imageService.imageConfig.ImageFace = ImageFace.Front;
        imgSource = this.imageService.imageConfig.ImageFrontContent;
    } else {
        this.imageService.imageConfig.ImageFace = ImageFace.Back;
        imgSource = this.imageService.imageConfig.ImageBackContent;
    }

    this.imageService.imageConfig.ImageSource = this.imageService.imageBase64Prefix + imgSource;

    this.loadImagesToPrint();

    this.resizeImage(this.imageService.imageConfig.ImageFit);
  }

  copyToClipboard() {
      this.sessionService.checkSessionStatus();

      const img: HTMLImageElement = this.renderer.createElement('img');

      // Check whether browser is IE or not
      if (!this.isInternetExplorer) {
          img.src = this.imageService.imageConfig.ImageSource;
      }

      this.renderer.appendChild(this.divToBeCopiedToClipboard, img);

      this.divToBeCopiedToClipboard.setAttribute('contenteditable', 'true');

      this.selectImage();

      this.document.execCommand('copy');

      this.windowService.nativeWindow.getSelection().removeAllRanges();

      this.divToBeCopiedToClipboard.removeAttribute('contenteditable');

      this.renderer.removeChild(this.divToBeCopiedToClipboard, img);
  }

  print() {
      if (!this.imageService.isImageReady) { return; }

      this.sessionService.checkSessionStatus();
      this.windowService.nativeWindow.print();
  }

  openDialogBox(template: TemplateRef<any>) {
      this.sessionService.checkSessionStatus();
      this.modalRef = this.modalService.show(template, {class: 'modal-lg'});
  }

  closeDialogBox() {
    this.modalRef.hide();
  }

  download() {

    if (this.imageService.isGeneralDcoument) {
        this.downloadPdfFile();
    } else {
        this.downloadImageFile();
    }

    this.closeDialogBox();
  }

  downloadPdfFile() {

    if (!this.imageService.documentPages || this.imageService.documentPages.length === 0) {
      throw new Error('General document has no page');
    }

    const doc = new jsPDF()

    doc.addImage(this.imageService.imageBase64Prefix + this.imageService.documentPages[0].FrontImage.Content, 'PNG', 15, 10, 180, 0);

    for (let i = 1; i < this.imageService.documentPages.length; i++) {
        doc.addPage();
        doc.addImage(this.imageService.imageBase64Prefix +
                     this.imageService.documentPages[i].FrontImage.Content, 'PNG', 15, 10, 180, 0);
    }

    doc.save('document.pdf');
  }

  downloadImageFile() {
      this.loadCanvasElement();

      this.canvas.toBlob(function(blob) {
          saveAs(blob, 'document.png');
      });
  }

  invertColour() {
      this.sessionService.checkSessionStatus();

      this.loadCanvasElement();

      const imageData = this.context.getImageData(0, 0, this.canvas.width, this.canvas.height);
      const data = imageData.data;

      for (let i = 0; i < data.length; i += 4) {
          data[i] = 255 - data[i];
          data[i + 1] = 255 - data[i + 1];
          data[i + 2] = 255 - data[i + 2];
      }

      this.context.putImageData(imageData, 0, 0);

      this.imageService.imageConfig.ImageSource = this.canvas.toDataURL();
  }

  enterFullscreen() {
    this.isFullScreenButtonDisabled  = true;

    this.sessionService.checkSessionStatus();

    this.tokenService.checkTokenStatus();

    this.imageService.notifyLoadImageStatus(true);
  }

  exitFullscreen() {
      this.windowService.closeWindow();
  }

  resizeImage(shape: string) {
    this.sessionService.checkSessionStatus();

    this.imageService.resizeImage(shape);

    this.imageService.notifyWhenImageShapeChanged();
  }

  private handleFullscreenAction() {
    timer(5000).subscribe(() => this.isFullScreenButtonDisabled = false );

    this.imageService.notifyLoadImageStatus(false);

    const basePath = 'viewimage/home';

    if (this.imageService.isDefaultImageRequest) {
      this.openDefaultImageFullscreen(basePath);
     } else {
      this.openFullScreen(basePath);
     }
  }

  private openDefaultImageFullscreen(basePath: string) {
    const defaultUrl = `${basePath}?fullscreen=true&default=true&scan=${this.imageService.isScannedImageRequest}`;
    this.windowService.openFullScreen(defaultUrl);
  }

  private openFullScreen(basePath: string) {
    const type = this.activatedRoute.snapshot.paramMap.get('type');
    const seqNumber = this.activatedRoute.snapshot.paramMap.get('seqNumber');

    const faceImage = this.imageService.imageConfig.ImageFace;
    const pageNumber = this.imageService.imageConfig.ImagePage;

    const params = `${this.host}/${type}/${this.irn}/${seqNumber}`;

    let queryParams = `faceImage=${faceImage}&fullscreen=true&pageNumber=${pageNumber}`;

    if (this.isDocumentNameEditable) {
        queryParams += `&editable=${this.isEditable}&source=${this.moduleName}`;
    }

    this.windowService.openFullScreen(`${basePath}/${params}?${queryParams}`);
  }

  private loadImagesToPrint() {

    if (!this.imageService.isImageReady) { return; }

    this.printDiv.innerHTML = '';

    this.loadImageToPrint(this.imageService.imageConfig.ImageSource);

    if (this.imageService.isGeneralDcoument) {

        for (let i = 1; i < this.imageService.documentPages.length; i++) {
            this.loadImageToPrint(this.imageService.imageBase64Prefix + this.imageService.documentPages[i].FrontImage.Content);
        }
    }
  }

  private loadImageToPrint(imgSource: string) {

    const p: HTMLImageElement = this.renderer.createElement('p');

    const img: HTMLImageElement = this.renderer.createElement('img');

    if (!this.isFullscreen && this.isInternetExplorer) {
      img.width = 675;
    } else {
      img.width = 1000;
    }

    img.src = imgSource;

    p.className = 'pageBreak';

    this.renderer.appendChild(p, img);

    this.renderer.appendChild(this.printDiv, p);
  }

  private loadCanvasElement() {
    const img: HTMLImageElement = this.renderer.createElement('img');
    img.src = this.imageService.imageConfig.ImageSource;

    this.canvas = this.renderer.createElement('canvas');

    this.canvas.width = img.width || this.windowService.nativeWindow.outerWidth;
    this.canvas.height = img.height || this.windowService.nativeWindow.outerHeight;

    this.context = this.canvas.getContext('2d');
    this.context.drawImage(img, 0, 0);
  }

  private selectImage() {
    const selection = this.windowService.nativeWindow.getSelection();
    const range = this.document.createRange();

    range.selectNodeContents(this.divToBeCopiedToClipboard);
    selection.removeAllRanges();
    selection.addRange(range);
  }
}
