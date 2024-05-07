import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { User, UserType } from './model/user.model';
import { UserService } from './user.service';

@Injectable({
  providedIn: 'root',
})
export class ClientGuard implements CanActivate {
    user: User;
  constructor(private router: Router, private userService: UserService) {}

  canActivate(): boolean {
    const userTypeNumericValue: number = UserType.Client; 
    if (userTypeNumericValue === UserType.Client) {
      return true;
    } else {
      this.router.navigate(['/login']);
      return false;
    }
  }
}
