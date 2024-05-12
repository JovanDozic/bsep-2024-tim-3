import { Component, OnInit } from '@angular/core';
import { User} from '../model/user.model';
import { UserService } from '../user.service';
import { AdvertisementService } from 'src/features/advertisement/advertisement.service';
import { Advertisement } from 'src/features/advertisement/model/advertisement.model';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';

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

  constructor(private formBuilder: FormBuilder,private userService: UserService, private adservis: AdvertisementService) { }
  
  ngOnInit(): void {
    this.id = 6;
    this.userService.getUserById(this.id).subscribe(
      (user: User) => {
        this.client = user;
        console.log('User Details:', this.client);

      },
      error => {
        console.error('Error fetching user:', error);
      }
    );
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
      this.adsForClient = this.advertisements.filter(ad => ad.clientId === 6 && ad.status === 1);
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
        clientId: 6, // Update this with actual client ID
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
}
