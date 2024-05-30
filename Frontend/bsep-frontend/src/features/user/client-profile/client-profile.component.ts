import { Component, OnInit } from '@angular/core';
import { User} from '../model/user.model';
import { UserService } from '../user.service';
import { AdvertisementService } from 'src/features/advertisement/advertisement.service';
import { Advertisement } from 'src/features/advertisement/model/advertisement.model';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { TokenStorage } from '../jwt/token.service';
import { ChangePasswordRequest } from '../model/change-password-request.model';

@Component({
  selector: 'app-client-profile',
  templateUrl: './client-profile.component.html',
  styleUrls: ['./client-profile.component.css']
})
export class ClientProfileComponent implements OnInit {
  client: User;
  advertisements: Advertisement[] = [];
  adsForClient: Advertisement[] = [];
  adRequest: Advertisement;
  showPopup = false;
  id:number;
  adRequestForm: FormGroup;
  isGolden: boolean = false;
  deleted: boolean = false;
  showPopupPassword = false;
  changePasswordForm: FormGroup;

  constructor(private formBuilder: FormBuilder,private userService: UserService, private adservis: AdvertisementService, private tokenStorage: TokenStorage) { }
  
  ngOnInit(): void {
    this.id = 1;
    this.userService.getUserById(this.tokenStorage.getUserId()).subscribe(
      (user: User) => {
        this.client = user;
        console.log('User Details:', this.client);
        if(this.client.packageType == 2){
          this.isGolden = true
        }
      },
      error => {
        console.error('Error fetching user:', error);
      }
    );
    this.changePasswordForm = this.formBuilder.group({
      currentPassword: ['', [Validators.required]],
      newPassword: ['', [Validators.required]]
    });
    this.adRequestForm = this.formBuilder.group({
      description: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      deadline: ['', Validators.required]
    }, {
      validators: this.endDateAfterStartDateAndDeadlineValidator // Custom validator for end date after start date
    });

    this.adservis.getAllAdvertisements().subscribe(ads => {
      this.advertisements = ads;
      this.adsForClient = this.advertisements.filter(ad => ad.clientId === this.client.id && ad.status === 1);
    });

  }

  endDateAfterStartDateAndDeadlineValidator(formGroup: FormGroup) {
    const startDate = formGroup.get('startDate').value;
    const endDate = formGroup.get('endDate').value;
    const deadline = formGroup.get('deadline').value;
  
    if (deadline && startDate && endDate) {
      if (new Date(deadline) < new Date(startDate) && new Date(startDate) < new Date(endDate)) {
        return null;
      } else {
        return { endDateAfterStartDateAndDeadline: true };
      }
    }
  
    return null;
  }
  deleteData() : void {
    this.deleted = true
  }
  optionYes() : void {
    this.deleted = false
    this.userService.deleteData(this.client.id).subscribe(result => {
      if (result) {
        console.log("Your data deleted successfully");
      } else {

        console.log("Error in deleting data");
      }
    })
  }
  optionNo() : void {
    this.deleted = false
  }
  openPopup(): void {
    this.showPopup = true;
  }
  submitForm() {
    if (this.adRequestForm.valid) {
      const formData = this.adRequestForm.value;
      const advertisement: Advertisement = {
        id: 0,
        slogan: null,
        startDate: formData.startDate,
        endDate: formData.endDate,
        description: formData.description,
        clientId: this.client.id, // Update this with actual client ID
        deadline: formData.deadline,
        status: 0
      };
      this.adservis.createAdvertisement(advertisement).subscribe(result => {
        if (result) {
          this.showPopup = false;
          console.log("Advertisement created successfully");
        } else {

          console.log("Error in creating advertisement");
        }
      });
    } else {
      // Form is invalid
      console.log("Form is invalid");
    }
  }
  updateUser() {
    console.log("usao u funkciju");
    this.userService.updateUser(this.client).subscribe(
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
