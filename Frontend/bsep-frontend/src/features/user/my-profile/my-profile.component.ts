import { Component, OnInit } from '@angular/core';
import { UserType, User } from '../model/user.model';
import { UserService } from '../user.service';

@Component({
  selector: 'app-my-profile',
  templateUrl: './my-profile.component.html',
  styleUrls: ['./my-profile.component.css']
})
export class MyProfileComponent implements OnInit {
  user: User = {} as User;
  updatedUser!: User;

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.user.id = 1;
    this.user.name = "Luka";
    this.user.surname = "Melanko";
    this.user.address = "Sofije Paskovic 16";
    this.user.city = "Novi Sad";
    this.user.country = "Srbija";
    this.user.email = "luka@gmail.com";
    this.user.phone = "372364824";
    this.user.type = UserType.Client;
    /*this.userService.getCurrentUser().subscribe(
      (user: User) => {
        this.user = user;
        this.updatedUser = user;
      },
      (error) => {
        console.error('Error fetching current user:', error);
      }
    );*/
  }
  updateProfile(event: Event) {
    event.preventDefault();
  
    /*this.userService.updateCustomer(this.updatedCustomer).subscribe(
      (user: User) => {
        this.user = user;
        this.updatedUser = user;
        alert('Profile updated successfully.');
      }
    );*/
  }
}
