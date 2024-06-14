import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../user.service';
import { Credentials } from '../model/login.model';
import { environment } from 'src/app/env/environment';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  showPopupPassword = false;
  sendEmailForm: FormGroup;
  reCAPTCHAResponse: string | null = null;
  reCAPTCHASiteKey: string = environment.RECAPTCHA_SITE_KEY;

  constructor(
    private router: Router,
    private userService: UserService,
    private fb: FormBuilder
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });

    this.sendEmailForm = this.fb.group({
      email: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {}

  login(): void {
    const login: Credentials = {
      username: this.loginForm.value.username || '',
      password: this.loginForm.value.password || '',
      reCAPTCHAToken: this.reCAPTCHAResponse,
    };
    if (this.loginForm.valid && this.reCAPTCHAResponse) {
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

  enterEmail(): void {
    this.showPopupPassword = true;
  }

  sendEmail(): void {
    if(this.sendEmailForm.valid) {
      const formData = this.sendEmailForm.value;
      this.userService.requestPasswordReset(formData).subscribe(result => {
        if(result) {
          this.showPopupPassword = false;
          console.log("Email for password reset sent successfully");
        } else {
          console.log("Error");
        }
      });
    } else {
      console.log("Form is invalid");
    }
  }

  onReCAPTCHAResolved(response: string | null): void {
    if (response) {
      this.reCAPTCHAResponse = response;
      // console.log('reCAPTCHA resolved with response: ', response);
    }
    else {
      console.log('reCAPTCHA resolved with error');
      alert('reCAPTCHA resolved with error');
    }
  }
}
