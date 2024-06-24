import { ImageViewerComponent } from '../components/imageViewer/image-viewer.component';
import { ErrorPageComponent } from '../components/error-page/error-page.component';
import { Routes as Routes_1 } from "@angular/router";

export const Routes: Routes_1 = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },

    { path: 'viewimage/home/:host/:type/:irn/:seqNumber', component: ImageViewerComponent },
    { path: 'viewimage/home/:host/:type/:irn', component: ImageViewerComponent },

    { path:  'home/:host/:type/:irn/:seqNumber', component: ImageViewerComponent },
    { path:  'home/:host/:type/:irn', component: ImageViewerComponent },

    { path: 'home', component: ImageViewerComponent },
    { path: 'viewimage/home', component: ImageViewerComponent },

    { path: 'error-page/:errorType', component: ErrorPageComponent },
    { path: '**',   component: ErrorPageComponent }
];
