import { Component, OnInit } from '@angular/core';
import { User } from '../model/user.model';
import { UserService } from '../user.service';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { TokenStorage } from '../jwt/token.service';
import { ChangePasswordRequest } from '../model/change-password-request.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-profile',
  templateUrl: './admin-profile.component.html',
  styleUrls: ['./admin-profile.component.css']
})
export class AdminProfileComponent implements OnInit {

  admin: User;
  allClients: User[] = [];
  allEmployees: User[] = [];
  allUsers: User[] = [];
  id:number;
  showPopup = false;
  registerForm: FormGroup;
  blockedUser: User;
  showPopupPassword = false;
  changePasswordForm: FormGroup;

  constructor(private formBuilder: FormBuilder,private userService: UserService, private tokenStorage: TokenStorage,private router: Router) { }

  ngOnInit(): void {
    this.id = 3;
    this.userService.getUserById(this.tokenStorage.getUserId()).subscribe(
      (user: User) => {
        this.admin = user;
        console.log('User Details:', this.admin);

      },
      error => {
        console.error('Error fetching user:', error);
      }
    )
    this.loadUsers();

    this.changePasswordForm = this.formBuilder.group({
      currentPassword: ['', [Validators.required]],
      newPassword: ['', [Validators.required, this.passwordValidator()]],
      newPasswordAgain: ['', [Validators.required]]
   }, {
      validator: this.MustMatch('newPassword', 'newPasswordAgain')
   });
    

    this.registerForm = this.formBuilder.group({
      email: ['', Validators.required],
      password: ['', Validators.required],
      firstname: ['', Validators.required],
      lastname: ['', Validators.required],
      address: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      phone: ['', Validators.required],
      role: ['', Validators.required]
    });
  
  }
  loadUsers() {
    this.userService.getUnblocked().subscribe(users => {
      this.allUsers = users;
      this.allClients = this.allUsers.filter(user => user.role === 0);
      this.allEmployees = this.allUsers.filter(user => user.role === 1);
    });
  }
  openPopup(): void {
    this.showPopup = true;
  }
  closePopup() : void {
    this.showPopupPassword = false;
  }

  openPopupPassword(): void {
    this.showPopupPassword = true;
  }
  changePassword() {
    if (this.changePasswordForm.valid) {
      const changePasswordRequest: ChangePasswordRequest = {
        userId: this.tokenStorage.getUserId(),
        oldPassword: this.changePasswordForm.value.currentPassword,
        newPassword: this.changePasswordForm.value.newPassword
      };

      this.userService.changePassword(changePasswordRequest).subscribe(
        (response: boolean) => { 
          if(response) {
          console.log("Password changed successfully");
          this.router.navigate(['/login']);
        } else {
          console.error("Failed to change password");
        }
        this.showPopupPassword = false;
      },
      error => {
        console.error("Error changing password:", error);
        this.showPopupPassword = false;
      }
      );
    } else {
      console.log("Form is invalid");
    }
  }

  togglePasswordVisibility(fieldId: string): void {
    const passwordField = document.getElementById(fieldId) as HTMLInputElement;
    if (passwordField.type === 'password') {
        passwordField.type = 'text'; // Show password
        // Update button text to "Hide"
        const button = document.querySelector(`button[data-field="${fieldId}"]`) as HTMLButtonElement;
        if (button) {
            button.innerText = 'Hide';
        }
    } else {
        passwordField.type = 'password'; // Hide password
        // Update button text to "Show"
        const button = document.querySelector(`button[data-field="${fieldId}"]`) as HTMLButtonElement;
        if (button) {
            button.innerText = 'Show';
        }
    }
}


  blockUser(user: User) {
    this.blockedUser = user;
    this.userService.blockUser(this.blockedUser).subscribe(
      (updated: boolean) => {
          if (updated) {
            this.loadUsers();
              console.log('User blocked successfully');
          } else {
              console.error('Failed to block user');
          }
      },
      error => {
          console.error('Error blocking user:', error);
      }
  );
  
  }

  submitForm() {
    if(this.registerForm.valid){
      const formData = this.registerForm.value;
      const user: User = {
        id:0,
        email: formData.email,
        password: formData.password,
        firstname: formData.firstname,
        lastname: formData.lastname,
        address: formData.address,
        city: formData.city,
        country: formData.country,
        phone: formData.phone,
        companyName: "Advertisement company",
        taxId: null,
        packageType: 0,
        clientType: 0,
        role: formData.role
      };
      this.userService.register(user).subscribe(result => {
        if(result) {
          this.showPopup = false;
          this.loadUsers();
          console.log("User registered successfully");
        } else {
          console.log("Error");
        }
      });
    } else {
      console.log("Form is invalid");
    }
  }
  updateUser() {
    console.log("usao u funkciju");
    this.userService.updateUser(this.admin).subscribe(
        (updated: boolean) => {
            if (updated) {
                console.log('Employee updated successfully');
            } else {
                console.error('Failed to update employee');
            }
        },
        error => {
            console.error('Error updating employee:', error);
        }
    );
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

 passwordValidator(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } | null => {
    const value = control.value;
    if (!value) {
      return null;
    }
    const hasUpperCase = /[A-Z]/.test(value);
    const hasLowerCase = /[a-z]/.test(value);
    const hasNumber = /[0-9]/.test(value);
    const hasSpecialCharacter = /[-._@+!?]/.test(value);
    const isValidLength = value.length >= 16;
    const passwordValid = hasUpperCase && hasLowerCase && hasNumber && hasSpecialCharacter && isValidLength;
    return !passwordValid ? { passwordInvalid: true } : null;
  };
}
}
