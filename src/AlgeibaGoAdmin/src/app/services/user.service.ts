import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ProfileUser } from '../models/profileuser.model';


@Injectable()

export class UserService {

    constructor(private http: HttpClient) { }

    getUserInfoFromEndpoint(url: string, token: string) {

        return this.http.get<ProfileUser>(url, {
            headers: {
                "Authorization": 'Bearer ' + token
            }
        });
    }
}