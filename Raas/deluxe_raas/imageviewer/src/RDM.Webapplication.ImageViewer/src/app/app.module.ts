import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { ModalModule } from 'ngx-bootstrap/modal';
import { NgxImageZoomModule } from 'ngx-image-zoom';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSliderModule } from '@angular/material/slider';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AngularDraggableModule } from 'angular2-draggable';
import { ToastrModule } from 'ngx-toastr';
import { TruncateModule } from '@yellowspot/ng-truncate';

import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { ImageViewerComponent } from './components/imageViewer/image-viewer.component';
import { ErrorPageComponent } from './components/error-page/error-page.component';
import { ToolbarComponent } from './components/toolbar/toolbar.component';
import { ToolbarPaginationComponent } from './components/toolbar-pagination/toolbar-pagination.component';
import { ToolbarDocumentNameComponent } from './components/toolbar-document-name/toolbar-document-name.component';
import { ToolbarButtonsComponent } from './components/toolbar-buttons/toolbar-buttons.component';

import { MouseWheelDirective } from './directives/mousewheel.directive';

import { Routes } from './constants/routes';

import { JwtInterceptor } from './interceptors/jwt.interceptor';
import { ErrorInterceptor } from './interceptors/error.interceptor';

import { WindowService } from './services/window.service';
import { ImageService } from './services/image.service';
import { ConfigService } from './services/config.service';
import { SessionService } from './services/session.service';
import { TokenService } from './services/token.service';
import { ActivityService } from './services/activity.service';

import { CookieService } from "ngx-cookie-service";

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    ImageViewerComponent,
    ErrorPageComponent,
    ToolbarComponent,
    ToolbarPaginationComponent,
    ToolbarDocumentNameComponent,
    ToolbarButtonsComponent,
    MouseWheelDirective,
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    BrowserAnimationsModule,
    MatButtonModule,
    MatToolbarModule,
    AngularDraggableModule,
    MatSliderModule,
    MatIconModule,
    MatTooltipModule,
    MatProgressSpinnerModule,
    TruncateModule,
    NgxImageZoomModule,
    ModalModule.forRoot(),
    ToastrModule.forRoot(
      {
        positionClass: 'toast-bottom-left',
        preventDuplicates: true,
        closeButton: true,
        progressBar: true,
        tapToDismiss: true
      }
    ),
    RouterModule.forRoot(Routes, {
    initialNavigation: 'enabledBlocking',
    scrollPositionRestoration: 'enabled',
    anchorScrolling: 'enabled'
    })
  ],
  providers: [
    WindowService,
    ImageService,
    ConfigService,
    SessionService,
    TokenService,
    ActivityService,
    CookieService,
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
