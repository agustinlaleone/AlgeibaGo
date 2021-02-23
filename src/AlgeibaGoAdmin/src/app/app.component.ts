import { Component } from '@angular/core';
import { OAuthService, JwksValidationHandler } from 'angular-oauth2-oidc';
import { authConfig } from './services/authConfig';
import { AppConfigService } from './services/AppConfigService';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  constructor(private oauthService: OAuthService, private appConfigService: AppConfigService) {
    this.configureWithNewConfigApi();
  }

  private configureWithNewConfigApi() {

    authConfig.issuer = this.appConfigService.config.issuer;
    authConfig.redirectUri = this.appConfigService.config.redirectUri;
    authConfig.clientId = this.appConfigService.config.clientId;
    authConfig.scope = this.appConfigService.config.scope;
    authConfig.responseType = this.appConfigService.config.responseType;
    authConfig.postLogoutRedirectUri = this.appConfigService.config.apiBaseURL;
    this.oauthService.configure(authConfig);
    this.oauthService.tokenValidationHandler = new JwksValidationHandler();
    this.oauthService.loadDiscoveryDocumentAndTryLogin();
  }


}
