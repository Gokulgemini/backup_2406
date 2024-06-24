import { Component } from '@angular/core';

import { ActivityService } from '../../services/activity.service';
import { ImageService } from '../../services/image.service';
import { SessionService } from '../../services/session.service';

@Component({
  selector: 'toolbar-pagination',
  templateUrl: './toolbar-pagination.component.html',
  styleUrls: ['./toolbar-pagination.component.css']
})
export class ToolbarPaginationComponent {

  private currentPage = 1;

  constructor(private sessionService: SessionService,
              public imageService: ImageService,
              private activityService: ActivityService) { }

  get isFirstPage(): boolean {
    return this.imageService.imageConfig.ImagePage <= 1;
  }

  get isLastPage(): boolean {
    return this.imageService.imageConfig.ImagePage === this.imageService.imageConfig.ImageTotalPage;
  }

  changePageNumber() {

    if (this.currentPage === this.imageService.imageConfig.ImagePage) { return; }

    if (this.imageService.imageConfig.ImagePage < 1) {
        this.imageService.imageConfig.ImagePage = 1;
    }

    if (this.imageService.imageConfig.ImagePage > this.imageService.imageConfig.ImageTotalPage) {
        this.imageService.imageConfig.ImagePage = this.imageService.imageConfig.ImageTotalPage;
    }

    this.currentPage =  this.imageService.imageConfig.ImagePage;

    this.pageChanged();
  }

  firstPage() {
    if (this.isFirstPage) {
        return;
    }

    this.imageService.imageConfig.ImagePage = 1;
    this.pageChanged();
  }

  previousPage() {
    if (this.isFirstPage) {
        return;
    }

    this.imageService.imageConfig.ImagePage -= 1;
    this.pageChanged();
  }

  nextPage() {
    if (this.isLastPage) {
        return;
    }

    this.imageService.imageConfig.ImagePage += 1;
    this.pageChanged();
  }

  lastPage() {
    if (this.isLastPage) {
        return;
    }

    this.imageService.imageConfig.ImagePage = this.imageService.imageConfig.ImageTotalPage;
    this.pageChanged();
  }

  private pageChanged() {
    this.sessionService.checkSessionStatus();

    this.activityService.logActivity();

    this.imageService.notifyWhenPageChanged();
  }
}
