import { Component, OnInit } from '@angular/core';
import { User} from '../model/user.model';
import { Advertisement } from '../model/advertisement.model';
import { UserService } from '../user.service';

@Component({
  selector: 'app-client-profile',
  templateUrl: './client-profile.component.html',
  styleUrls: ['./client-profile.component.css']
})
export class ClientProfileComponent implements OnInit {
  client: User;
  advertisements: Advertisement[] = [];
  showPopup = false;
  id:number;

  constructor(private userService: UserService) { }
  
  ngOnInit(): void {
    this.id = 7;
    this.userService.getUserById(this.id).subscribe(
      (user: User) => {
        this.client = user;
        console.log('User Details:', this.client);

      },
      error => {
        console.error('Error fetching user:', error);
      }
    )
    this.advertisements = [
      { id: 1, slogan: 'Slogan 1', duration: 30, description: 'Description 1', clientId: 1 },
      { id: 2, slogan: 'Slogan 2', duration: 45, description: 'Description 2', clientId: 2 },
      { id: 3, slogan: 'Slogan 3', duration: 30, description: 'Description 1', clientId: 2 },
      { id: 4, slogan: 'Slogan 4', duration: 30, description: 'Description 1', clientId: 1 },
      { id: 5, slogan: 'Slogan 5', duration: 30, description: 'Description 1', clientId: 1 },
      { id: 1, slogan: 'Slogan 6', duration: 30, description: 'Description 1', clientId: 1 },

    ];
    this.advertisements = this.advertisements.filter(ad => ad.clientId === this.client.id);


  }

  openPopup(): void {
    this.showPopup = true;
  }
  submitButton(): void {
    this.showPopup = false;
  }
  updateUser() {
    console.log("usao u funkciju");
    this.userService.updateUser(this.client).subscribe(
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
