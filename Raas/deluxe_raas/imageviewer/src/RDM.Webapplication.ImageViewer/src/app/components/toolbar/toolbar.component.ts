import { Component, Inject, Input } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

import { ImageService } from '../../services/image.service';

import { TargetHost } from '../../constants/targetHost';


@Component({
  selector: 'toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.css']
})
export class ToolbarComponent {

    @Input('divToBeCopiedToClipboard') divToBeCopiedToClipboard;
    @Input('divToBePrinted') printDiv;

    constructor(private activatedRoute: ActivatedRoute,
                private imageService: ImageService,
                @Inject(DOCUMENT) public document: any) { }

    get isDocumentNameEditable(): boolean {
    return this.isWebClientHost && this.isEditable;
    }

    get isEditable(): boolean {
        return this.activatedRoute.snapshot.queryParamMap.get('editable') != null
            ? this.activatedRoute.snapshot.queryParamMap.get('editable').toLowerCase() === 'true'
            : false;
    }

    get isFullscreen(): boolean {
        return this.activatedRoute.snapshot.queryParamMap.get('fullscreen') != null;
    }

    get isWebClientHost(): boolean {
        return this.host === TargetHost.WebClient;
    }

    get showPaging(): boolean {
        return this.imageService.isGeneralDcoument;
    }

    get host(): string {
        return this.activatedRoute.snapshot.paramMap.get('host');
    }

    get irn(): string {
        return this.activatedRoute.snapshot.paramMap.get('irn');
    }

    get moduleName(): string {
        return this.activatedRoute.snapshot.queryParamMap.get('source');
    }
}
