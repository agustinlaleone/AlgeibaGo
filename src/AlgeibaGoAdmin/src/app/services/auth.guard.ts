import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';

@Injectable()

export class AuthGuard implements CanActivate {

    constructor(private router: Router, private oauthService: OAuthService) { }

    canActivate() {
        if (this.oauthService.hasValidAccessToken()) {
            return true;
        }

        this.oauthService.initImplicitFlow();
    }
}