import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';

import { ToolbarPaginationComponent } from './toolbar-pagination.component';

import { SessionService } from '../../services/session.service';
import { ImageService } from '../../services/image.service';
import { WindowService } from '../../services/window.service';

import { SessionServiceStub } from './../../stubs/session.service.stub';
import { ImageServiceStub } from '../../stubs/image.service.stub';
import { WindowServiceStub } from './../../stubs/window.service.stub';

 describe('ToolbarPaginationComponent', () => {
  let component: ToolbarPaginationComponent;
  let fixture: ComponentFixture<ToolbarPaginationComponent>;

  let imageService: ImageServiceStub;
  let sessionService: SessionServiceStub;
  let windowService: WindowServiceStub;

  beforeEach(() => {

    sessionService = new SessionServiceStub();
    imageService = new ImageServiceStub();
    windowService = new WindowServiceStub();

    TestBed.configureTestingModule({
        imports: [
            FormsModule
        ],
        declarations: [
          ToolbarPaginationComponent
        ],
        providers: [
            { provide: SessionService, useValue: sessionService },
            { provide: ImageService, useValue: imageService },
            { provide: WindowService, useValue: windowService },
        ]
    }).compileComponents();

    fixture = TestBed.createComponent(ToolbarPaginationComponent);
    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should go to first page', () => {
    imageService.imageConfig.ImagePage = 5;

    component.firstPage();

    expect(imageService.imageConfig.ImagePage).toEqual(1);
  });

  it('should NOT go to first page when the user is already on first page', () => {
    spyOn(sessionService, 'checkSessionStatus');

    imageService.imageConfig.ImagePage = 1;
    imageService.imageConfig.ImageTotalPage = 5;

    component.firstPage();

     expect(sessionService.checkSessionStatus).not.toHaveBeenCalled();
  });

  it('should go to previous page', () => {
    imageService.imageConfig.ImagePage = 5;

    component.previousPage();

    expect(imageService.imageConfig.ImagePage).toEqual(4);
  });

  it('should stay on first page when the user is already on first page', () => {
    imageService.imageConfig.ImagePage = 1;

    component.previousPage();

    expect(imageService.imageConfig.ImagePage).toEqual(1);
  });

  it('should stay on last page when the user is already on last page', () => {
    imageService.imageConfig.ImagePage = 5;
    imageService.imageConfig.ImageTotalPage = 5;

    component.nextPage();

    expect(imageService.imageConfig.ImagePage).toEqual(5);
  });

  it('should go to next page', () => {
    imageService.imageConfig.ImagePage = 2;
    imageService.imageConfig.ImageTotalPage = 5;

    component.nextPage();

    expect(imageService.imageConfig.ImagePage).toEqual(3);
  });

  it('should go to last page', () => {
    imageService.imageConfig.ImagePage = 2;
    imageService.imageConfig.ImageTotalPage = 5;

    component.lastPage();

    expect(imageService.imageConfig.ImagePage).toEqual(imageService.imageConfig.ImageTotalPage);
  });

  it('should NOT go to last page', () => {
    spyOn(sessionService, 'checkSessionStatus');

    imageService.imageConfig.ImagePage = 5;
    imageService.imageConfig.ImageTotalPage = 5;

    component.lastPage();

     expect(sessionService.checkSessionStatus).not.toHaveBeenCalled();
  });

  it('should change page number',  async () => {
    imageService.imageConfig.ImagePage = 1;

    const dePageNumber = fixture.debugElement.query(By.css('#pageNumber'));
    const txtPageNumber: HTMLInputElement = dePageNumber.nativeElement;

    const newPage = 3;
    txtPageNumber.value = newPage.toString();

    txtPageNumber.dispatchEvent(new Event('input'));

    await fixture.whenStable();
    fixture.detectChanges();

    expect(imageService.imageConfig.ImagePage).toBe(newPage);
  });

  it('should NOT change page number when input page number is the current page', async () => {
    spyOn(sessionService, 'checkSessionStatus');

    imageService.imageConfig.ImagePage = 1;

    component.changePageNumber();

    expect(sessionService.checkSessionStatus).not.toHaveBeenCalled();
  });

  it('should go to first page when user inputs a page number less than 1', () => {
    imageService.imageConfig.ImagePage = 0;
    imageService.imageConfig.ImageTotalPage = 5;

    component.changePageNumber();

    expect(imageService.imageConfig.ImagePage).toEqual(1);
  });

  it('should go to last page when user inputs a page number grater than last page', () => {
    imageService.imageConfig.ImagePage = 6;
    imageService.imageConfig.ImageTotalPage = 5;

    component.changePageNumber();

    expect(imageService.imageConfig.ImagePage).toEqual(imageService.imageConfig.ImageTotalPage);
  });
});
