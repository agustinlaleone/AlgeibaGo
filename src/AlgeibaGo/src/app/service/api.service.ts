import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Route } from '../models/route.model';
import { AppConfigService } from '../service/AppConfigService';
import { DeviceData } from '../models/devicedata.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  version: String;
  baseUrl: String;
  apiIPUrl: string;
  apiCountryUrl: string;
  backOffice: string;
  apiKey : string;

  constructor(private environment: AppConfigService, private _http: HttpClient) { 
    this.baseUrl = environment.config.baseUrl; 
    this.version = environment.config.version;
    this.apiIPUrl = environment.config.apiIPUrl;
    this.apiCountryUrl = environment.config.apiCountryUrl;
    this.backOffice = environment.config.backOffice;
    this.apiKey = environment.config.apiCountryKey;
  }

  // Routes
  getRouteByName(name: String) {
    let url = this.baseUrl + "/" + this.version + '/RoutesPublic/GetByName/' + name;
    return this._http.get<Route>(url);
  }

  // RouteVisit
  registerRouteVisit(data: DeviceData) {
    
    let url = this.baseUrl + "/" + this.version + '/RoutesPublic/registerRouteVisit';
    return this._http.post(url, data);
  }

  getIpAddress(){
    return this._http.get<{ip: String}>(this.apiIPUrl);
  }

  getCountry(ip: String){
    let url = this.apiCountryUrl + "apiKey=" + this.apiKey + "&ip=" + ip;
    return this._http.get<{country_name: String}>(url);
  }

  getBackofficeURL(){
    return this.backOffice;
  }

}
