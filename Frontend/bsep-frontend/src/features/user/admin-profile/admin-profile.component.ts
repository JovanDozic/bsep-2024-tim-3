import { Component, OnInit } from '@angular/core';
import { User } from '../model/user.model';
import { UserService } from '../user.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

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

  constructor(private formBuilder: FormBuilder,private userService: UserService) { }

  ngOnInit(): void {
    this.id = 9;
    this.userService.getUserById(this.id).subscribe(
      (user: User) => {
        this.admin = user;
        console.log('User Details:', this.admin);

      },
      error => {
        console.error('Error fetching user:', error);
      }
    )
    
    this.userService.getAllUsers().subscribe(users => {
      this.allUsers = users;
      this.allClients = this.allUsers.filter(user => user.role === 0);
      this.allEmployees = this.allUsers.filter(user => user.role === 1);
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

  openPopup(): void {
    this.showPopup = true;
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
      this.userService.registerUser(user).subscribe(result => {
        if(result) {
          this.showPopup = false;
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
}
