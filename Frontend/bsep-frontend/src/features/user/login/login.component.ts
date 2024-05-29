import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../user.service';
import { Credentials } from '../model/login.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  loginForm: FormGroup;

  constructor(
    private router: Router,
    private userService: UserService,
    private fb: FormBuilder
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  login(): void {
    const login: Credentials = {
      username: this.loginForm.value.username || '',
      password: this.loginForm.value.password || '',
    };
    if (this.loginForm.valid) {
      this.userService.login(login).subscribe({
        next: () => {
          const user = this.userService.user$.getValue();
        },
      });
    }
  }

  register(): void {
    this.router.navigate(['/register']);
  }

  passwordless(): void {
    this.router.navigate(['/login-passwordless']);
  }
}
