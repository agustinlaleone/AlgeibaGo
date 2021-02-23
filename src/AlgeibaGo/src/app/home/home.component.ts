import { Component, OnInit, Injector, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { DOCUMENT } from '@angular/platform-browser';
import { ApiService } from '../service/api.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(@Inject(DOCUMENT) private document: Document, private service: ApiService, private injector: Injector) {
    this.redirectBackOffice();
   }

  ngOnInit() {
  }

  redirectBackOffice() {
    let url = this.service.getBackofficeURL();
    this.document.location.href = url;
  }
}
