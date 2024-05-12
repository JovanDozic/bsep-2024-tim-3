import { Component, OnInit } from '@angular/core';
import { UserService } from '../user.service';
import { concat } from 'rxjs';
import { User } from '../model/user.model';
import { Advertisement } from 'src/features/advertisement/model/advertisement.model';
import { AdvertisementService } from 'src/features/advertisement/advertisement.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

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

  constructor(private formBuilder: FormBuilder,private userService: UserService, private adService: AdvertisementService) { }

  ngOnInit(): void {
    this.id = 6;
    this.userService.getUserById(this.id).subscribe(
      (user: User) => {
        this.employee = user;
        console.log('User Details:', this.employee);

      },
      error => {
        console.error('Error fetching user:', error);
      }
    );
    this.loadAdvertisements();

    this.addSloganForm = this.formBuilder.group({
      slogan: ['', Validators.required]
    })
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

}
