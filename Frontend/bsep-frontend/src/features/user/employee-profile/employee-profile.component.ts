import { Component, OnInit } from '@angular/core';
import { UserService } from '../user.service';
import { concat } from 'rxjs';
import { User } from '../model/user.model';
import { Advertisement } from 'src/features/advertisement/model/advertisement.model';
import { AdvertisementService } from 'src/features/advertisement/advertisement.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TokenStorage } from '../jwt/token.service';
import { ChangePasswordRequest } from '../model/change-password-request.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-employee-profile',
  templateUrl: './employee-profile.component.html',
  styleUrls: ['./employee-profile.component.css']
})
export class EmployeeProfileComponent implements OnInit {

  employee: User;
  advertisements: Advertisement[] = [];
  adRequests: Advertisement[] = [];
  id:number;
  showPopup = false;
  addSloganForm: FormGroup;
  updatedAd: Advertisement;
  showPopupPassword = false;
  changePasswordForm: FormGroup;

  constructor(private formBuilder: FormBuilder,private userService: UserService, private adService: AdvertisementService, private tokenStorage: TokenStorage,private router: Router) { }

  ngOnInit(): void {
    this.id = 2;
    this.userService.getUserById(this.tokenStorage.getUserId()).subscribe(
      (user: User) => {
        this.employee = user;
        console.log('User Details:', this.employee);

      },
      error => {
        console.error('Error fetching user:', error);
      }
    );
    this.loadAdvertisements();
    this.changePasswordForm = this.formBuilder.group({
      currentPassword: ['', [Validators.required]],
      newPassword: ['', [Validators.required]],
      newPasswordAgain: ['', [Validators.required]]
   }, {
      validator: this.MustMatch('newPassword', 'newPasswordAgain')
   });
   


    this.addSloganForm = this.formBuilder.group({
      slogan: ['', Validators.required]
    })
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
   

  loadAdvertisements(): void {
    this.adService.getAllAdvertisements().subscribe(ads => {
      this.advertisements = ads.filter(ad => ad.status === 1);
      this.adRequests = ads.filter(ad => ad.status === 0);
    });
  }

  openPopup(ad: Advertisement): void {
    this.showPopup = true;
    this.updatedAd = ad;
    console.log(this.updatedAd.description);
  }

  closePopup() {
    this.showPopupPassword = false;
  }

  submitForm() {
    if (this.addSloganForm.valid) {
      const formData = this.addSloganForm.value;
      const advertisement: Advertisement = {
        id: this.updatedAd.id,
        slogan: formData.slogan,
        startDate: this.updatedAd.startDate,
        endDate: this.updatedAd.endDate,
        description: this.updatedAd.description,
        clientId: this.updatedAd.clientId,
        deadline: this.updatedAd.deadline,
        status: 1
      };
      this.adService.updateAdvertisement(advertisement).subscribe(result => {
        if(result) {
          this.showPopup = false;
          this.loadAdvertisements();
          console.log("Advertisement created successfully");
        } else {
          console.log("Error in createing advertisement");
        }
      });
    } else {
      console.log("Form is invalid");
    }

  }
  
  updateUser() {
    console.log("usao u funkciju");
    this.userService.updateUser(this.employee).subscribe(
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
}
