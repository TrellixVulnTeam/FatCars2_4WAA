import { Component, OnInit } from '@angular/core';
import { UserService } from '../Services/User.service';

@Component({
  selector: 'app-Users',
  templateUrl: './Users.component.html',
  styleUrls: ['./Users.component.scss']
})
export class UsersComponent implements OnInit {


  users: any;

  constructor(private userService: UserService) {
  }

  ngOnInit() {
    this.getUsers();
  }

  getUsers(){
      this.userService.getUser().subscribe( response => {
        this.users =  response;
      }, error => {
        console.log(error);
      });
    }
}

