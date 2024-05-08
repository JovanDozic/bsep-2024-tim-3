import { Component, OnInit } from '@angular/core';
import { UserType, User } from '../model/user.model';
import { UserService } from '../user.service';
import { Advertisement } from '../model/advertisement.model';

@Component({
  selector: 'app-employee-profile',
  templateUrl: './employee-profile.component.html',
  styleUrls: ['./employee-profile.component.css']
})
export class EmployeeProfileComponent implements OnInit {

  employee: User;
  advertisements: Advertisement[] = [];

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.employee = {
      id: 1,
      name: 'Luka',
      surname: 'Zelovic',
      email: 'zelovic.luka@example.com',
      password: 'password',
      address: '123 Main St',
      city: 'City',
      country: 'Country',
      phone: '123-456-7890',
      type: UserType.Employee // Assuming the employee is a natural person
    };
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

}
