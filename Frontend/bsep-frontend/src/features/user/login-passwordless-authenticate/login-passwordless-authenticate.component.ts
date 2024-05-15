import { Component, OnInit } from '@angular/core';
import { UserService } from '../user.service';
import { ActivatedRoute, Router } from '@angular/router';
import { EmailTokenRequest } from '../model/passwordless-token-request.model';

@Component({
  selector: 'app-login-passwordless-authenticate',
  templateUrl: './login-passwordless-authenticate.component.html',
  styleUrls: ['./login-passwordless-authenticate.component.css'],
})
export class LoginPasswordlessAuthenticateComponent implements OnInit {
  isLoading: boolean = false;
  constructor(
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.isLoading = true;
    this.route.queryParams.subscribe((params) => {
      var token: EmailTokenRequest = {
        token: params['token'],
      };

      if (token) {
        token.token = decodeURIComponent(token.token);

        this.userService.authenticatePasswordlessToken(token).subscribe({
          next: (response) => {
            this.isLoading = false;
          },
          error: (error) => {
            this.isLoading = false;
            alert(
              'Oops! Something went wrong with passwordless token! Please try again!'
            );
            this.router.navigate(['/login']);
          },
        });
      }
    });
  }
}
