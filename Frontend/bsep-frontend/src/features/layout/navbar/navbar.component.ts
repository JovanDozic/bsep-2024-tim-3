import { Component } from '@angular/core';
import { Route, Router } from '@angular/router';
import { UserService } from 'src/features/user/user.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {

  constructor(private router: Router, private service: UserService) {}

  navigateToProfile() : void {
    this.router.navigate(['/my-profile'])
  }
  logout() : void {
    this.service.logout();
    this.router.navigate(['/login'])
  }
}
