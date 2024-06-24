import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterTestingModule } from '@angular/router/testing';

import { ToolbarComponent } from './toolbar.component';
import { ToolbarButtonsComponent } from './../toolbar-buttons/toolbar-buttons.component';
import { ToolbarPaginationComponent } from './../toolbar-pagination/toolbar-pagination.component';
import { ToolbarDocumentNameComponent } from './../toolbar-document-name/toolbar-document-name.component';

import { ImageType } from '../../constants/imageType';
import { TargetHost } from '../../constants/targetHost';

import { CookieService } from "ngx-cookie-service";
import { WindowService } from '../../services/window.service';
import { ImageService } from '../../services/image.service';
import { SessionService } from '../../services/session.service';

import { ImageServiceStub } from '../../stubs/image.service.stub';
import { ActivatedRouteStub } from './../../stubs/activateRoute.stub';
import { SessionServiceStub } from './../../stubs/session.service.stub';
import { WindowServiceStub } from './../../stubs/window.service.stub';

import { ModalModule } from 'ngx-bootstrap/modal';
import { TruncateModule } from '@yellowspot/ng-truncate';
import { ToastrModule } from 'ngx-toastr';

 describe('ToolbarComponent', () => {
  let component: ToolbarComponent;
  let fixture: ComponentFixture<ToolbarComponent>;

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
            FormsModule,
            MatButtonModule,
            MatToolbarModule,
            MatIconModule,
            MatTooltipModule,
            TruncateModule,
            ModalModule.forRoot(),
            ToastrModule.forRoot()
        ],
        declarations: [
          ToolbarComponent,
          ToolbarDocumentNameComponent,
          ToolbarPaginationComponent,
          ToolbarButtonsComponent
        ],
        providers: [
          CookieService,
            { provide: WindowService, useValue: windowService },
            { provide: SessionService, useValue: sessionService },
            { provide: ImageService, useValue: imageService },
            { provide: ActivatedRoute, useValue: activatedRoute }
        ]
    }).compileComponents();

    fixture = TestBed.createComponent(ToolbarComponent);
    component = fixture.componentInstance;

    activatedRoute = fixture.debugElement.injector.get(ActivatedRoute) as any;
    activatedRoute.testParamMap = { type: ImageType.Check };
    activatedRoute.testQueryParamMap = { fullscreen: true };

    fixture.detectChanges();
  });

  it('should show the pagination div when the image type is GeneralDocument', () => {
    imageService.imageConfig.ImageType = ImageType.GeneralDocument;

    activatedRoute.testParamMap = { host: TargetHost.ITMS };

    fixture.detectChanges();

    const dePaginationDiv = fixture.debugElement.query(By.css('#btns-paging'));
    const divPagination: HTMLElement = dePaginationDiv.nativeElement;

    expect(divPagination).not.toBeNull();
  });

  it('should NOT show the pagination div when the image type is Remittance', () => {
    imageService.imageConfig.ImageType = ImageType.Remittance;

    fixture.detectChanges();

    const dePaginationDiv = fixture.debugElement.query(By.css('#btns-paging'));

    expect(dePaginationDiv).toBeNull();
  });

  it('should NOT show the pagination div when the image type is Check', () => {
    imageService.imageConfig.ImageType = ImageType.Check;

    fixture.detectChanges();

    const dePaginationDiv = fixture.debugElement.query(By.css('#btns-paging'));

    expect(dePaginationDiv).toBeNull();
  });

  it('should display document name edit button ', () => {
    imageService.imageConfig.ImageType = ImageType.GeneralDocument;
    imageService.imageConfig.ImageName = 'some name';

    activatedRoute.testQueryParamMap = { editable: 'true' };
    activatedRoute.testParamMap = { host: TargetHost.WebClient };

    fixture.detectChanges();

    const deEditDocName = fixture.debugElement.query(By.css('#editDocumentName'));
    const btnEditDocName: HTMLButtonElement = deEditDocName.nativeElement;

    expect(btnEditDocName).not.toBeNull();
  });

  it('should NOT display document name edit button ', () => {
    imageService.imageConfig.ImageType = ImageType.GeneralDocument;
    imageService.imageConfig.ImageName = 'some name';

    activatedRoute.testParamMap = { host: TargetHost.ITMS };

    fixture.detectChanges();

    const deEditDocName = fixture.debugElement.query(By.css('#editDocumentName'));

    expect(deEditDocName).toBeNull();
  });

  it('should not allow user to edit document name', () => {
    activatedRoute.testParamMap = { host: TargetHost.WebClient };
    activatedRoute.testQueryParamMap = { editable: 'false' };

    expect(component.isDocumentNameEditable).toBeFalsy();
  });

  it('should allow user to edit document name', () => {
    activatedRoute.testParamMap = { host: TargetHost.WebClient };
    activatedRoute.testQueryParamMap = { editable: 'true' };

    expect(component.isDocumentNameEditable).toBeTruthy();
  });
});
