import { Component, OnInit } from '@angular/core';
import { UserType, User } from '../model/user.model';
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

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.admin = {
      id: 1,
      name: 'Luka',
      surname: 'Zelovic',
      email: 'zelovic.luka@example.com',
      password: 'password',
      address: '123 Main St',
      city: 'City',
      country: 'Country',
      phone: '123-456-7890',
      type: UserType.Admin // Assuming the employee is a natural person
    };
    /*
    this.userService.GetAllUsers().subscribe({
      next: (result: PagedResult<User>) => {
        this.allUsers = result.results;
      },
      error: (err:any) => {
        console.log(err);
      }
    });
    */

    const users: User[] = [
      { id: 2, name: 'John', surname: 'Doe', email: 'john.doe@example.com', password: 'password', address: '456 Elm St', city: 'City', country: 'Country', phone: '987-654-3210', type: UserType.Client },
      { id: 3, name: 'Jane', surname: 'Smith', email: 'jane.smith@example.com', password: 'password', address: '789 Oak St', city: 'City', country: 'Country', phone: '123-789-4560', type: UserType.Employee },
      { id: 3, name: 'Spae', surname: 'Smith', email: 'jane.smith@example.com', password: 'password', address: '789 Oak St', city: 'City', country: 'Country', phone: '123-789-4560', type: UserType.Employee },
    ];

    this.allUsers = users;

    this.allUsers.forEach(user => {
      if (user.type === UserType.Client) {
        this.allClients.push(user);
      } else if (user.type === UserType.Employee) {
        this.allEmployees.push(user);
      }
    });

  }
}
