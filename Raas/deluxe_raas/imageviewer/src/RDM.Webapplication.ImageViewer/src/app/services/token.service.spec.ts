import { TestBed, inject } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { CookieService } from "ngx-cookie-service";
import { TokenService } from './token.service';
import { ImageService } from './image.service';
import { WindowService } from './window.service';

import { ImageServiceStub } from '../stubs/image.service.stub';
import { WindowServiceStub } from '../stubs/window.service.stub';

describe('TokenService', () => {

    let windowService: WindowServiceStub;
    let imageService: ImageServiceStub;

    beforeEach(() => {

        windowService = new WindowServiceStub();
        imageService = new ImageServiceStub();

        TestBed.configureTestingModule({
            imports: [RouterTestingModule.withRoutes(
                [
                  {path: 'error-page/401', redirectTo: ''}
                ]
              )] ,
            declarations: [],
            providers: [
                TokenService,
                CookieService,
                { provide: WindowService, useValue: windowService },
                { provide: ImageService, useValue: imageService }
            ]
        }).compileComponents();
    });

    it('should create token service', inject([TokenService, CookieService], (tokenService: TokenService, cookieService: CookieService) => {
        expect(tokenService).toBeTruthy();
    }));

    it('should store SU token in local storage', inject([TokenService, CookieService], (tokenService: TokenService, cookieService: CookieService) => {
        tokenService.storeSUToken('XYZ');

        expect(cookieService.get('su_token')).toBe('XYZ');
    }));

    it('should remove SU token cookie item', inject([TokenService, CookieService], (tokenService: TokenService, cookieService: CookieService) => {
        tokenService.storeSUToken('XYZ');

        tokenService.removeSUToken();

        expect(cookieService.get('su_token')).toBe('');
    }));

    it('should clear all cookie items when SU token is empty', inject([TokenService, CookieService], (tokenService: TokenService, cookieService: CookieService) => {
        tokenService.storeSUToken('');

        expect(cookieService.get('su_token')).toBe('');
        expect(cookieService.get('access_token')).toBe('');
    }));

    it('should clear all cookie items', inject([TokenService, CookieService], (tokenService: TokenService, cookieService: CookieService) => {
        tokenService.storeSUToken('XYZ');
        cookieService.set('access_token', 'ABCDE');

        tokenService.clearTokens();

        expect(cookieService.get('su_token')).toBe('');
        expect(cookieService.get('access_token')).toBe('');
    }));
});
