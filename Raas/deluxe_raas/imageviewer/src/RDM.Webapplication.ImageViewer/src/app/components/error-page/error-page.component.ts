import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { ErrorType } from 'src/app/constants/errorType';

@Component({
    selector: 'error-page',
    templateUrl: 'error-page.component.html'
})
export class ErrorPageComponent implements OnInit  {
    title: string;
    message: string;

    constructor(private activatedRoute: ActivatedRoute) {}

    ngOnInit() {
        const errorType = +this.activatedRoute.snapshot.paramMap.get('errorType') || ErrorType.PageNotFound;

        switch (errorType) {

            case ErrorType.Unauthorized:
                this.title = 'Error accessing image';
                this.message = 'To continue, please refresh the page and open the image again.';
            break;

            default:
                this.title = 'The image is not available at this time';
                this.message = 'Please try again later.';
            break;
        }
    }
}
