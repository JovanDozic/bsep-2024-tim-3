import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { User } from '../model/user.model';
import { UserService } from '../user.service';
import { Observable, catchError, map } from 'rxjs';
import { TokenStorage } from '../jwt/token.service';

@Injectable({
  providedIn: 'root',
})
export class ClientGuard implements CanActivate {
    user: User = null;
    userId:number;

  constructor(private router: Router, private userService: UserService, private tokenStorage: TokenStorage) {
    this.userId = 1;
  }

  canActivate(): Observable<boolean> {
    return this.userService.getUserById(this.tokenStorage.getUserId()).pipe(
      map((user: User) => {
        this.user = user;
        console.log(this.user.role);
        console.log('User Details:', this.user);
        if (user.role === 0) {
            return true;
        }
        this.userService.logout();
        this.router.navigate(['/login']);
        return false;
      }),
      catchError(error => {
        console.error('Error fetching user:', error);
        return [false]; // Return false in case of error
      })
    );
  }

}
