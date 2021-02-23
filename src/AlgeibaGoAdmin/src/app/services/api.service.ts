import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Route } from '../models/route.model';
import { AppConfigService } from '../services/AppConfigService';
import { RelRouteUser } from '../models/relrouteuser.model';
import { UsersByRoute } from '../models/usersbyroute.model';
import { User } from '../models/user.model';
import { Observable } from 'rxjs';
import { RouteDataChart } from '../models/dataChart';


@Injectable({
  providedIn: 'root'
})
export class ApiService {

  headers_object = new HttpHeaders({
    'Content-Type': 'application/json',
    'Authorization': "Bearer " + sessionStorage.getItem("access_token")
  });
  httpOptions = {
    headers: this.headers_object
  };

  version: String;
  baseUrl: String;

  constructor(private environment: AppConfigService, private _http: HttpClient) {
    this.version = environment.config.version;
    this.baseUrl = environment.config.apiBaseURL;
  }

  findRoutes(userId, role, filter, sortOrder, pageIndex, pageSize, method) {
    let url = '';
    switch (method) {
      case 1: {
        url = this.baseUrl + "/" + this.version + '/Routes/GetPaged';
        break;
      }
      case 2: {
        url = this.baseUrl + "/" + this.version + '/Routes/GetOwnPaged';
        break;
      }
      case 3: {
        url = this.baseUrl + "/" + this.version + '/Routes/GetOthersPaged';
        break;
      }
      case 4: {
        url = this.baseUrl + "/" + this.version + '/Routes/GetMyFavoritesLink';
        break;
      }
    };

    return this._http.post<UsersByRoute[]>(url, { 'user': userId, 'role': role, 'filter': filter, 'sortOrder': sortOrder, 'pageIndex': pageIndex, 'pageSize': pageSize },
      this.httpOptions);
  }

  getRoutes(userID: String, roleName: String) {
    var model = { userID, roleName };
    let url = this.baseUrl + "/" + this.version + '/Routes';
    return this._http.post<UsersByRoute[]>(url, model, this.httpOptions);
  }

  getRoutesCount(filter, method) {
    let url = '';
    switch (method) {
      case 1: {
        url = this.baseUrl + "/" + this.version + '/Routes/GetCount';
        break;
      }
      case 2: {
        url = this.baseUrl + "/" + this.version + '/Routes/GetOwnCount';
        break;
      }
      case 3: {
        url = this.baseUrl + "/" + this.version + '/Routes/GetOthersCount';
        break;
      }
    };
    let userId = sessionStorage.getItem('userId');
    return this._http.post<number>(url, { 'filter': filter, 'user': userId }, this.httpOptions);
  }

  postRoute(objRoute: Route) {
    let url = this.baseUrl + "/" + this.version + '/Routes';
    return this._http.post(url, objRoute, this.httpOptions);
  }

  postRelRouteUser(objRelRouteUser: RelRouteUser) {
    let url = this.baseUrl + "/" + this.version + '/RelRouteUsers';
    return this._http.post(url, objRelRouteUser, this.httpOptions);
  }

  putRoute(objRoute: Route) {
    let url = this.baseUrl + "/" + this.version + '/Routes';
    return this._http.put(url, objRoute, this.httpOptions);
  }

  deleteRoute(objRoute: Route) {
    let url = this.baseUrl + "/" + this.version + '/Routes/' + objRoute.Id;
    return this._http.delete(url, this.httpOptions);
  }

  deleteRelRouteUser(objRoute: Route) {
    let url = this.baseUrl + "/" + this.version + '/RelRouteUsers/' + objRoute.Id;
    return this._http.delete(url, this.httpOptions);
  }

  findRelations(userId, role, filter, sortOrder, pageIndex, pageSize) {
    let url = this.baseUrl + "/" + this.version + '/RelRouteUser/GetPaged';
    return this._http.post<UsersByRoute[]>(url, { 'user': userId, 'role': role, 'filter': filter, 'sortOrder': sortOrder, 'pageIndex': pageIndex, 'pageSize': pageSize },
      this.httpOptions);
  }

  findOwnRelations(userId, role, filter, sortOrder, pageIndex, pageSize) {
    let url = this.baseUrl + "/" + this.version + '/RelRouteUser/GetOwnPaged';
    return this._http.post<UsersByRoute[]>(url, { 'user': userId, 'role': role, 'filter': filter, 'sortOrder': sortOrder, 'pageIndex': pageIndex, 'pageSize': pageSize },
      this.httpOptions);
  }

  findOthersRelations(userId, role, filter, sortOrder, pageIndex, pageSize) {
    let url = this.baseUrl + "/" + this.version + '/RelRouteUser/GetOthersPaged';
    return this._http.post<UsersByRoute[]>(url, { 'user': userId, 'role': role, 'filter': filter, 'sortOrder': sortOrder, 'pageIndex': pageIndex, 'pageSize': pageSize },
      this.httpOptions);
  }

  getUserRoles(userId: string) {
    let url = this.baseUrl + "/" + this.version + "/Roles?userId=" + userId;
    return this._http.get<string[]>(url, this.httpOptions);
  }

  getUsers() {
    let url = this.baseUrl + "/Users";
    return this._http.get<User[]>(url, this.httpOptions);
  }

  findUsers(userId, role, filter, sortOrder, pageIndex, pageSize) {
    let url = this.baseUrl + "/" + this.version + "/Users/GetPaged";
    return this._http.post<User[]>(url, { 'user': userId, 'role': role, 'filter': filter, 'sortOrder': sortOrder, 'pageIndex': pageIndex, 'pageSize': pageSize },
      this.httpOptions);
  }

  getUsersCount(filter) {
    let url = this.baseUrl + "/" + this.version + '/Users/GetCount';
    return this._http.post<number>(url, { 'filter': filter }, this.httpOptions);
  }

  getAllUsers(): Observable<User[]> {
    let url = this.baseUrl + "/" + this.version + '/Users/GetAllUsers';
    return this._http.get<User[]>(url, this.httpOptions);
  }

  saveRelations(relations: User[], idRoute: number) {
    var model = { relations, idRoute };
    let url = this.baseUrl + "/" + this.version + '/RelRouteUsers/SaveRelations';
    return this._http.post(url, model, this.httpOptions);
  }

  saveRoles(user: User) {
    let url = this.baseUrl + "/" + this.version + '/Roles/SaveRoles';
    return this._http.post(url, user, this.httpOptions);
  }

  getAllRoles() {
    let url = this.baseUrl + "/" + this.version + '/Roles/GetAll';
    return this._http.get<string[]>(url, this.httpOptions);
  }

  deleteUser(userId: string) {
    let url = this.baseUrl + "/" + this.version + '/Users?userId=' + userId;
    return this._http.delete(url, this.httpOptions);
  }

  addUser(user: User) {
    let url = this.baseUrl + "/" + this.version + '/Users';
    return this._http.post(url, user, this.httpOptions);
  }

  updateUser(user: User) {
    let url = this.baseUrl + "/" + this.version + '/Users';
    return this._http.put(url, user, this.httpOptions);
  }

  changePassword(user: User) {
    let url = this.baseUrl + "/" + this.version + '/Users/ChangePassword';
    return this._http.post(url, user, this.httpOptions);
  }

  getDataChartCountry(id: number) {
    let url = this.baseUrl + "/" + this.version + '/Routes/GetStatiticsCountry?id=' + id;
    return this._http.get<RouteDataChart[]>(url, this.httpOptions);
  }

  getDataChartClicks(data: RelRouteUser) {
    let url = this.baseUrl + "/" + this.version + '/Routes/GetStatiticsClicks';
    return this._http.post<RouteDataChart[]>(url, data, this.httpOptions);
  }
  
  updateFavorites(data: RelRouteUser) {
    let url = this.baseUrl + "/" + this.version + '/Routes/UpdateFavorites';
    return this._http.put<RouteDataChart[]>(url, data, this.httpOptions);
  }
}