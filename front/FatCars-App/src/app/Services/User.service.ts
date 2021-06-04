import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  baseUrl = 'https://localhost:5001/api/User';

constructor(private http: HttpClient) { }
    getUser(){
        return this.http.get(this.baseUrl);
    }
}
