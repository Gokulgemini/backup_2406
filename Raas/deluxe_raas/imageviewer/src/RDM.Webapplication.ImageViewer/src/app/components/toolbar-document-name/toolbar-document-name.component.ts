import { Component, OnInit, OnDestroy, Input, ViewEncapsulation } from '@angular/core';
import { Subscription  } from 'rxjs';

import { ImageService } from '../../services/image.service';
import { WindowService } from '../../services/window.service';
import { SessionService } from '../../services/session.service';

import { ToastrService } from 'ngx-toastr';

import { MessageType } from '../../constants/messageType';

import { Message } from '../../vo/message';

@Component({
  selector: 'toolbar-document-name',
  templateUrl: './toolbar-document-name.component.html',
  styleUrls: ['./toolbar-document-name.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class ToolbarDocumentNameComponent implements OnInit, OnDestroy {

  @Input('irn') irn: string;
  @Input('moduleName') moduleName: string;
  @Input('isFullscreen') isFullscreen: boolean;
  @Input('isDocumentNameEditable') isDocumentNameEditable: boolean;

  isDocumentEditOn = false;

  private documentNameOriginal: string;
  private subscriptionDocNameChange: Subscription;

  constructor(public imageService: ImageService,
              private windowService: WindowService,
              private sessionService: SessionService,
              private toastrService: ToastrService) { }

  get DocumentNameCharacterLimit(): number {
      return 10 ;
  }

  get disableDocNameLableTooltip(): boolean {
    return this.imageService.imageConfig.ImageName.length <= this.DocumentNameCharacterLimit;
  }

  ngOnInit() {
    this.subscriptionDocNameChange = this.imageService.notifyDocNameChange$
      .subscribe( docName => this.imageService.imageConfig.ImageName = docName );
  }

  ngOnDestroy() {
    this.subscriptionDocNameChange.unsubscribe();
  }

  editDocumentName() {
    this.sessionService.checkSessionStatus();

    this.isDocumentEditOn = true;
    this.documentNameOriginal = this.imageService.imageConfig.ImageName;
  }

  cancelEditDocumentName() {
    this.isDocumentEditOn = false;
    this.imageService.imageConfig.ImageName = this.documentNameOriginal;
  }

  saveDocumentName() {
    this.sessionService.checkSessionStatus();

    if (!this.imageService.imageConfig.ImageNameIsValid()) {
        this.toastrService.error('Document name must have up to 50 characters');
        return;
    }

   this.imageService.updateDocumentName(this.irn, this.imageService.imageConfig.ImageName, this.moduleName)
    .subscribe(
        () => {

            this.windowService.postMessageToChild(new Message(MessageType.DocumentNameChange, this.imageService.imageConfig.ImageName));

            this.isDocumentEditOn = false;
            this.toastrService.success('Document name changed successfully.');
        },
        () => {
            this.cancelEditDocumentName();
            this.toastrService.error('Error saving changes');
        }
    );
  }

  setDocumentNameTextDisableAttribute(): string {
      return !this.isDocumentEditOn ? '' : null;
  }
}
