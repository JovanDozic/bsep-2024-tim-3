import { Component, OnInit } from '@angular/core';
import { UserService } from '../user.service';
import { Router } from '@angular/router';
import { User } from '../model/user.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  registrationForm: FormGroup;
  isIndividual: boolean = true;
  namePlaceholder: string = 'First Name';
  /*
  name: string = '';
  surnameOrPIB: string = '';
  email: string = '';
  password: string = '';
  confirmPassword: string = '';
  address: string = '';
  city: string = '';
  country: string = '';
  phoneNumber: string = '';
  isNatural: boolean = true;
  type: UserType = UserType.Client;
  user: User[] = [];*/

  selectedOption: string = 'natural';

  constructor(
    private userService: UserService,
    private router: Router,
    private formBuilder: FormBuilder
  ) {}

  public onRadioChange(): void {
    this.isIndividual = !this.isIndividual;
  }

  getPlaceholder(isIndividual: boolean): string {
    return isIndividual ? 'First Name' : 'Company Name';
  }

  ngOnInit() {
    this.registrationForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(16),
          Validators.pattern(
            /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[-._@+!?]).{16,}$/
          ),
        ],
      ],
      confirmPassword: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: [''],
      companyName: [''],
      taxId: [''],
      city: ['', Validators.required],
      country: ['', Validators.required],
      phone: ['', Validators.required],
      packageType: ['', Validators.required]
    });
    /*this.userService.getUsers().subscribe(
      (result)=>{
        this.users = result;
      }
    )*/
  }

  onChange(): void {
    /*
    if(this.selectedOption == "natural") {
      this.isNatural = true
    } else {
      this.isNatural = false
    }*/
  }

  register(): void {
    if (
      this.registrationForm.value.password !==
      this.registrationForm.value.confirmPassword
    ) {
      alert('Passwords do not match');
    } else {
      let type: number;
      if (this.isIndividual) {
        type = 0;
      } else{
        type = 1;
      }
      let user: User = {
        id: 0,
        email: this.registrationForm.value.email,
        password: this.registrationForm.value.password,
        address: this.registrationForm.value.address,
        city: this.registrationForm.value.city,
        country: this.registrationForm.value.country,
        phone: this.registrationForm.value.phone,
        packageType: parseInt(this.registrationForm.value.packageType),
        clientType: type,
        role: 0,
      };
    }
  }

  onConfirmPasswordInput(): void {
    /*
    const registerButton: HTMLButtonElement | null = document.getElementById('registerButton') as HTMLButtonElement | null;

    if (registerButton) {
      registerButton.disabled = this.password !== this.confirmPassword;
    }
  }*/
  }
}
