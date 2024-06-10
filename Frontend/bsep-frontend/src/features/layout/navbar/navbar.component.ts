import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TokenStorage } from 'src/features/user/jwt/token.service';
import { User } from 'src/features/user/model/user.model';
import { UserService } from 'src/features/user/user.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {

  isAdmin: boolean = false;
  loggedUser: User;

  constructor(private router: Router, private service: UserService, private tokenStorage: TokenStorage) {}

  ngOnInit(): void {
    this.loadUser();
  }

  navigateToProfile(): void {
    this.router.navigate(['/my-profile']);
  }

  logout(): void {
    this.service.logout();
    this.router.navigate(['/login']);
  }

  loadUser() {
    this.service.getUserById(this.tokenStorage.getUserId()).subscribe(
      (user: User) => {
        this.loggedUser = user;
        this.isAdmin = user.role === 2;
        console.log(this.isAdmin);
      },
      error => {
        console.error('Error fetching user:', error);
      }
    );
  }
  
  logs(): void {
    this.router.navigate(['/logs']);
  }

  async rateLimiterTest(packageType: string, quantity: number): Promise<void> {
    let isLimitAchieved = false;
    for (let i = 0; i < quantity; ++i) {
        if (isLimitAchieved) {
            break;
        }
        try {
            await new Promise<void>((resolve, reject) => {
                this.service.testRateLimiter(packageType).subscribe(
                    (data) => {
                        console.log("Request OK");
                        resolve();
                    },
                    (error) => {
                        alert('Limit achieved! Try again in 30 seconds.');
                        isLimitAchieved = true;
                        reject(error);
                    }
                );
            });
        } catch (error) {
            console.error('Error occurred:', error);
            break;
        }
    }
}
}
