import { Injectable, isDevMode } from '@angular/core';

/**
 * This gets rolled into the main.HASH.js when built for prod
 * DevOps can target the variable replacement tags in the js and replace it for deployment
 * If other properies are needed please follow baseUrl as the example
 */
@Injectable()
export class ConfigService {
    get baseUrl() {
        return isDevMode() ? 'https://localhost:44312' : '#{RDM.API.Address.HTTPS}/imageviewer';
    }
}
