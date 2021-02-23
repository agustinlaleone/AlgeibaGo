import { Component, OnInit } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { ProfileUser } from '../models/profileuser.model';
import { Router } from '@angular/router';
import { UserService } from '../services/user.service';
UserService
@Component({
  selector: 'app-callback',
  templateUrl: './callback.component.html',
  styleUrls: ['./callback.component.css']
})
export class CallbackComponent implements OnInit {

  constructor(private oauthService: OAuthService, private router: Router, private serviceLogin: UserService) { }

  profileUser: ProfileUser;

  ngOnInit() {
    this.oauthService.loadDiscoveryDocumentAndTryLogin().then(_ => {
      if (!this.oauthService.hasValidIdToken() || !this.oauthService.hasValidAccessToken()) {
        this.oauthService.initImplicitFlow('some-state');
      } else {
        this.serviceLogin.getUserInfoFromEndpoint(this.oauthService.userinfoEndpoint, this.oauthService.getAccessToken()).subscribe(rest => {
          this.profileUser = new ProfileUser();
          this.profileUser = rest;
          sessionStorage.setItem('userDni', this.profileUser.name);
          sessionStorage.setItem('userMail', this.profileUser.email);
          sessionStorage.setItem('userFullName', this.profileUser.email);
          sessionStorage.setItem('userId', this.profileUser.sub);
          sessionStorage.setItem('role', this.profileUser.role[0]);
          if (sessionStorage.getItem('role').length == 1) {
            sessionStorage.setItem('role', this.profileUser.role.toString());
          }
          this.router.navigate(['/']);
        });
      }
    });

  }



}
