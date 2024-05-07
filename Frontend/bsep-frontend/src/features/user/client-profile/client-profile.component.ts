import { Component, OnInit } from '@angular/core';
import { User, UserType } from '../model/user.model';
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

  constructor(private userService: UserService) { }
  
  ngOnInit(): void {
    this.client = {
      id: 1,
      name: 'Luka',
      surname: 'Zelovic',
      email: 'zelovic.luka@example.com',
      password: 'password',
      address: '123 Main St',
      city: 'City',
      country: 'Country',
      phone: '123-456-7890',
      type: UserType.Client
    };
    /* 
    this.userService.getAllAds().subscribe({
      next:(result:)
    })
    */
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

}
