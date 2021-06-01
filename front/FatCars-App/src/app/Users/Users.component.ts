import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-Users',
  templateUrl: './Users.component.html',
  styleUrls: ['./Users.component.scss']
})
export class UsersComponent implements OnInit {

  users: any;

  constructor(private http: HttpClient) {
  }

  ngOnInit() {
    this.getUsers();
  }

  getUsers(){
      this.http.get('http://localhost:5000/api/User').subscribe( response => {
        this.users =  response;
      }, error => {
        console.log(error);
      });
  }

}
