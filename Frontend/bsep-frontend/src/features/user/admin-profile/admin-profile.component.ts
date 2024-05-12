import { Component, OnInit } from '@angular/core';
import { User } from '../model/user.model';
import { UserService } from '../user.service';

@Component({
  selector: 'app-admin-profile',
  templateUrl: './admin-profile.component.html',
  styleUrls: ['./admin-profile.component.css']
})
export class AdminProfileComponent implements OnInit {

  admin: User;
  allClients: User[] = [];
  allEmployees: User[] = [];
  allUsers: User[] = [];
  id:number;

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.id = 9;
    this.userService.getUserById(this.id).subscribe(
      (user: User) => {
        this.admin = user;
        console.log('User Details:', this.admin);

      },
      error => {
        console.error('Error fetching user:', error);
      }
    )
    
    this.userService.getAllUsers().subscribe(users => {
      this.allUsers = users;
      this.allClients = this.allUsers.filter(user => user.role === 0);
      this.allEmployees = this.allUsers.filter(user => user.role === 1);
    });
  
  }
  updateUser() {
    console.log("usao u funkciju");
    this.userService.updateUser(this.admin).subscribe(
        (updated: boolean) => {
            if (updated) {
                console.log('Employee updated successfully');
            } else {
                console.error('Failed to update employee');
            }
        },
        error => {
            console.error('Error updating employee:', error);
        }
    );
}
}
