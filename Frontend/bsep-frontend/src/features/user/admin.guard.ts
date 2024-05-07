import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { User, UserType } from './model/user.model';
import { UserService } from './user.service';

@Injectable({
  providedIn: 'root',
})
export class EmployeeGuard implements CanActivate {
    user: User;
  constructor(private router: Router, private userService: UserService) {}

  canActivate(): boolean {
    /*
    this.userService.getUser(this.tokenStorage.getId()).subsribe({
        next: (result: User) => {
            user = result;
        }
    })
    */

    const userTypeNumericValue: number = UserType.Admin; // user.type
    if (userTypeNumericValue === UserType.Admin) {
      return true;
    } else {
      this.router.navigate(['/login']);
      return false;
    }
  }
}
