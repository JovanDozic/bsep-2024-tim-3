import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../user.service';
import { Credentials } from '../model/login.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-passwordless',
  templateUrl: './login-passwordless.component.html',
  styleUrls: ['./login-passwordless.component.css'],
})
export class LoginPasswordlessComponent implements OnInit {
  loginForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
    });
  }

  ngOnInit(): void {}

  sendPasswordlessLink() {
    const login: Credentials = {
      username: this.loginForm.value.username,
      password: '',
      isPasswordless: true,
    };
    this.userService.sendPasswordlessLink(login).subscribe({
      next: () => {
        alert('Passwordless link sent!');
        this.router.navigate(['/home']);
      },
      error: (err) => {
        alert('Error sending passwordless link! Please try again!');
      },
    });
  }
}
