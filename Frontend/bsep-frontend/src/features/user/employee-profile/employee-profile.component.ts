import { Component, OnInit } from '@angular/core';
import { UserService } from '../user.service';
import { Advertisement } from '../model/advertisement.model';
import { concat } from 'rxjs';
import { User } from '../model/user.model';

@Component({
  selector: 'app-employee-profile',
  templateUrl: './employee-profile.component.html',
  styleUrls: ['./employee-profile.component.css']
})
export class EmployeeProfileComponent implements OnInit {

  employee: User;
  advertisements: Advertisement[] = [];
  id:number;

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.id = 6;
    this.userService.getUserById(this.id).subscribe(
      (user: User) => {
        this.employee = user;
        console.log('User Details:', this.employee);

      },
      error => {
        console.error('Error fetching user:', error);
      }
    )
    
    this.advertisements = [
      { id: 1, slogan: 'Slogan 1', duration: 30, description: 'Description 1', clientId: 1 },
      { id: 2, slogan: 'Slogan 2', duration: 45, description: 'Description 2', clientId: 2 },
      { id: 3, slogan: 'Slogan 1', duration: 30, description: 'Description 1', clientId: 1 },
      { id: 4, slogan: 'Slogan 1', duration: 30, description: 'Description 1', clientId: 1 },
      { id: 5, slogan: 'Slogan 1', duration: 30, description: 'Description 1', clientId: 1 },
      { id: 1, slogan: 'Slogan 1', duration: 30, description: 'Description 1', clientId: 1 },
      // Add more advertisements here
    ];
  }
  updateUser() {
    console.log("usao u funkciju");
    this.userService.updateUser(this.employee).subscribe(
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
