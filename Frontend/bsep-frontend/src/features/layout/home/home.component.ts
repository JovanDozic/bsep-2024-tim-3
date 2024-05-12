import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { User } from 'src/features/user/model/user.model';
import { UserService } from 'src/features/user/user.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  loggedUser: User;
  allUsers: User[];
  isAdmin: boolean = false;
  isClient: boolean = false;
  isEmployee: boolean = false;

  constructor(private userService: UserService, private router: Router){}

  ngOnInit() : void {
    this.userService.getUserById(1).subscribe(
      (user: User) => {
        this.loggedUser = user;
        console.log('User Details:', this.loggedUser);
        this.isLoggedIn();
      },
      error => {
        console.error('Error fetching user:', error);
      }
    )
    
    this.userService.getAllUsers().subscribe(users => {
      this.allUsers = users;
      //this.allClients = this.allUsers.filter(user => user.role === 0);
      //this.allEmployees = this.allUsers.filter(user => user.role === 1);
    },
    error => {
      console.error('Greska kod dobavljanja svih:', error);
    });
  }

  isLoggedIn() : void {
    if(this.loggedUser.role == 2){
      this.isAdmin = true;
    } else if (this.loggedUser.role == 1){
      this.isEmployee == true;
    } else {
      this.isClient == true;
    }
  }

  roleManagement(userId: number) : void {
    console.log("userId: " + userId)
    this.router.navigate(['/role-management', userId]);
  }

  permissionManagement(permissionId: number) : void {
    console.log("permissionId: " + permissionId)
    this.router.navigate(['/permission-management', permissionId]);
  }
}
