import { ComponentFixture, TestBed, inject } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { MatTooltipModule } from '@angular/material/tooltip';

import { throwError } from 'rxjs';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { TruncateModule } from '@yellowspot/ng-truncate';

import { ToolbarDocumentNameComponent } from './toolbar-document-name.component';

import { WindowService } from '../../services/window.service';
import { ImageService } from '../../services/image.service';
import { SessionService } from '../../services/session.service';

import { ImageServiceStub } from '../../stubs/image.service.stub';
import { SessionServiceStub } from './../../stubs/session.service.stub';
import { WindowServiceStub } from './../../stubs/window.service.stub';


 describe('ToolbarComponent', () => {
  let component: ToolbarDocumentNameComponent;
  let fixture: ComponentFixture<ToolbarDocumentNameComponent>;

  let imageService: ImageServiceStub;
  let sessionService: SessionServiceStub;
  let windowService: WindowServiceStub;

  beforeEach(() => {

    imageService = new ImageServiceStub();
    sessionService = new SessionServiceStub();
    windowService = new WindowServiceStub();

    TestBed.configureTestingModule({
        imports: [
            FormsModule,
            MatTooltipModule,
            TruncateModule,
            ToastrModule.forRoot()
        ],
        declarations: [
          ToolbarDocumentNameComponent
        ],
        providers: [
            { provide: WindowService, useValue: windowService },
            { provide: SessionService, useValue: sessionService },
            { provide: ImageService, useValue: imageService }
        ]
    }).compileComponents();

    fixture = TestBed.createComponent(ToolbarDocumentNameComponent);
    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should disable document name textbox', () => {
    component.cancelEditDocumentName();

    const docnameTextboxDisableAtrr = component.setDocumentNameTextDisableAttribute();

    expect(docnameTextboxDisableAtrr).toBe('');
  });

  it('should enable document name textbox', () => {
    component.editDocumentName();

    const docnameTextboxDisableAtrr = component.setDocumentNameTextDisableAttribute();

    expect(docnameTextboxDisableAtrr).toBeNull();
  });

  it('should cancel dcoument name changes', () => {
    imageService.imageConfig.ImageName = 'Document 1';
    const docNameBeforeChange = imageService.imageConfig.ImageName;

    component.editDocumentName();

    imageService.imageConfig.ImageName = 'Document Updated';

    component.cancelEditDocumentName();

    expect(component.isDocumentEditOn).toBeFalsy();
    expect(imageService.imageConfig.ImageName).toEqual(docNameBeforeChange);
  });

  it('should save dcoument name changes', inject([ToastrService], (toastrService: ToastrService) => {

    spyOn(toastrService, 'success');

    imageService.imageConfig.ImageName = 'Document 1';

    component.editDocumentName();

    imageService.imageConfig.ImageName = 'Document Updated';
    const docNameAfterChange = imageService.imageConfig.ImageName;

    component.saveDocumentName();

    expect(component.isDocumentEditOn).toBeFalsy();
    expect(imageService.imageConfig.ImageName).toEqual(docNameAfterChange);
    expect(toastrService.success).toHaveBeenCalledWith('Document name changed successfully.');

  }));

  it('should NOT save dcoument name change if it has more than 50 characters', inject([ToastrService], (toastrService: ToastrService) => {

    spyOn(toastrService, 'error');

    imageService.imageConfig.ImageName = 'Document 1';

    component.editDocumentName();

    imageService.imageConfig.ImageName = 'Document Document Document Document Document Document';

    component.saveDocumentName();

    expect(component.isDocumentEditOn).toBeTruthy();
    expect(toastrService.error).toHaveBeenCalledWith('Document name must have up to 50 characters');

  }));

  it('should display an error message if there is a fault on the backend side', inject([ToastrService], (toastrService: ToastrService) => {

    spyOn(imageService, 'updateDocumentName').and.returnValue(throwError(new Error('Fake error')));
    spyOn(toastrService, 'error');

    imageService.imageConfig.ImageName = 'Document 1';
    const docNameBeforeChange = imageService.imageConfig.ImageName;

    component.editDocumentName();

    imageService.imageConfig.ImageName = 'Document Updated';

    component.saveDocumentName();

    expect(component.isDocumentEditOn).toBeFalsy();
    expect(imageService.imageConfig.ImageName).toEqual(docNameBeforeChange);
    expect(toastrService.error).toHaveBeenCalledWith('Error saving changes');
  }));

  it('should be notified when doc name is updated', () => {
    const docName = 'doc name updated';

    imageService.notifyChildrenWhenDocNameChanged(docName);

    expect(imageService.imageConfig.ImageName).toBe(docName);
  });

});
