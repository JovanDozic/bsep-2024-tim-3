import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../user.service';
import { ResetPassword } from '../model/resetPassword.model';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  token: string | null = null;
  changePasswordForm: FormGroup;
  passwordFieldType: string = 'password';
  passwordToggleText: string = 'Show';
  newPassword: string = "";

  constructor(private router: Router,private route: ActivatedRoute,private fb: FormBuilder, private userService: UserService) {
    this.changePasswordForm = this.fb.group({
      newPassword: ['', [Validators.required]],
      newPasswordAgain: ['', [Validators.required]]
    }, {
      validator: this.MustMatch('newPassword', 'newPasswordAgain')
    });
   }

   MustMatch(controlName: string, matchingControlName: string) {
    return (formGroup: FormGroup) => {
        const control = formGroup.controls[controlName];
        const matchingControl = formGroup.controls[matchingControlName];
   
        // return if another validator has already found an error on the matchingControl
        if (matchingControl.errors && !matchingControl.errors.mustMatch) {
            return;
        }
   
        // set error on matchingControl if validation fails
        if (control.value !== matchingControl.value) {
            matchingControl.setErrors({ mustMatch: true });
        } else {
            matchingControl.setErrors(null);
        }
    };
   }
  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.token = params['token'];
      console.log(this.token); // For debugging purposes
    });
  }

  changePassword() : void {
    if(this.changePasswordForm.valid) {
      const formData = this.changePasswordForm.value;
      this.newPassword = formData.newPassword;
      const resetPassword: ResetPassword = {
        token: this.token,
        newPassword: this.newPassword
      }
      console.log(resetPassword.token);
      console.log(resetPassword.newPassword);
      this.userService.resetPassword(resetPassword).subscribe(result => {
        if(result) {
          console.log("Changed password successfully")
          this.router.navigate(['/login']);
        }
        else {
          console.log("Error")
        }
      });
    } else {
      console.log("Form is invalid");
    }
  }

  togglePasswordVisibility(): void {
    if (this.passwordFieldType === 'password') {
      this.passwordFieldType = 'text';
      this.passwordToggleText = 'Hide';
    } else {
      this.passwordFieldType = 'password';
      this.passwordToggleText = 'Show';
    }
  }
}
