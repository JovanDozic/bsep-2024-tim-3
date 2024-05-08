import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { User, UserType } from '../model/user.model';
import { UserService } from '../user.service';

@Injectable({
  providedIn: 'root',
})
export class AdminGuard implements CanActivate {
    user: User;
  constructor(private router: Router, private userService: UserService) {
    this.user = {
      id: 1,
      name: 'Luka',
      surname: 'Zelovic',
      email: 'zelovic.luka@example.com',
      password: 'password',
      address: '123 Main St',
      city: 'City',
      country: 'Country',
      phone: '123-456-7890',
      type: UserType.Admin
    };
  }

  canActivate(): boolean {
    /*
    this.userService.getUser(this.tokenStorage.getId()).subsribe({
        next: (result: User) => {
            user = result;
        }
    })
    */

    if (this.user.type === UserType.Admin) {
      return true;
    } else {
      this.router.navigate(['/login']);
      return false;
    }
  }
}
