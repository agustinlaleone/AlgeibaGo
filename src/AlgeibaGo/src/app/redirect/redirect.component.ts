import { Component, OnInit, Injector, Inject } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ApiService } from '../service/api.service';
import { DeviceDetectorService } from 'ngx-device-detector';
import { DeviceData } from '../models/devicedata.model';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-redirect',
  templateUrl: './redirect.component.html',
  styleUrls: ['./redirect.component.css']
})
export class RedirectComponent implements OnInit {

  parametro: String;
  clientdata: DeviceData;
  ip : String;

  constructor(private route: ActivatedRoute, private router: Router, private injector: Injector, private service: ApiService, private deviceService: DeviceDetectorService) { 
    this.MapDeviceData();
  }

  ngOnInit() { 
  }

  saveVisitedRoute(){
    debugger;
    this.route.params.subscribe(params => {
      this.parametro = params.string;
      this.getRouteByName(this.parametro)
    }, error => {
      this.navigateRouter("servererror");
    });
  }

  navigateRouter(route: string){
    const router = this.injector.get(Router);
    router.navigate([route]);
  }

  getRouteByName(parametro: String){
    this.service.getRouteByName(parametro).subscribe((data) => {
      var redirectURL = data.RedirectUrl;
      if (data) {
       this.clientdata.routeId = data.Id;
       this.registerRouteVisited(redirectURL);
      }
  }, error => {
    this.navigateRouter("error");
  });
  }

  registerRouteVisited(redirectURL: string){
    this.service.registerRouteVisit(this.clientdata).subscribe();
       window.location.href = redirectURL;
  }
  
  getIPData() {
    return this.service.getIpAddress().subscribe(res => 
      {
        this.ip = res.ip;
        this.clientdata.ipData = this.ip;
        return this.getCountry();
      }, error => {
        this.clientdata.ipData = null;
      });
  }

  getCountry(){
    this.service.getCountry(this.ip).subscribe(res => 
      { 
        this.clientdata.country = res.country_name;
        this.saveVisitedRoute();
      }, error => {
        this.clientdata.country = null;
      });
  }
  MapDeviceData() {
    this.clientdata = new DeviceData();
    this.clientdata.browser = this.deviceService.getDeviceInfo().browser;
    this.clientdata.browserVersion = this.deviceService.getDeviceInfo().browser_version;
    this.clientdata.device = this.deviceService.getDeviceInfo().device;
    this.clientdata.os = this.deviceService.getDeviceInfo().os;
    this.clientdata.osVersion = this.deviceService.getDeviceInfo().os_version;
    this.clientdata.userAgent = this.deviceService.getDeviceInfo().userAgent;
    this.getIPData();
  }
}
