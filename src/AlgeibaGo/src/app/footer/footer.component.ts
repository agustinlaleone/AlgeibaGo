import { Component, OnInit } from '@angular/core';
import { AppConfigService } from '../service/AppConfigService';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent implements OnInit {

  public version: String;
  
  constructor(private environment: AppConfigService) { }

  ngOnInit() {
    this.version = this.environment.config.version;
  }

}
