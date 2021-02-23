import { Component, OnInit } from '@angular/core';
import { AppConfigService } from '../services/AppConfigService';
import { OAuthService } from 'angular-oauth2-oidc';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
  options = [
    { name: 'Action' },
    { name: 'Another action' },
    { name: 'Something else here' },
    { isDivider: true },
  ];
  image: string;
  title: string;
  nombreUsuario: string;
  constructor(private appConfigService: AppConfigService, private oauthService: OAuthService, private cookieService: CookieService) {
    this.image = "assets/Image/LogoAlgeibaGO.png";
    this.title = "AlgeibaGo";
    if (window.screen.width <= 600) { // 768px portrait
      this.image = "assets/Image/LogoAlgeibaGO.png";
      this.title = "AlgeibaGo";
    }
    this.nombreUsuario = sessionStorage.getItem('userFullName');
  }

  logOut() {
    this.cookieService.deleteAll('/');
    this.oauthService.logOut();
  }

}