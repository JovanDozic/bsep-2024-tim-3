import { Component, OnInit } from '@angular/core';
import { UserService } from '../user.service';
import { Router } from '@angular/router';
import { User } from '../model/user.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as bootstrap from 'bootstrap';
import { Verify2faRequest } from '../model/verify-2fa.model';

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
  qrCode: string = '';
  setup2faCode: string = '';
  verify2faRequest: Verify2faRequest = {
    code: '',
  };

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
      twoFactorAuth: [false],
      firstName: ['', Validators.required],
      lastName: [''],
      companyName: [''],
      taxId: [''],
      city: ['', Validators.required],
      address: ['', Validators.required],
      country: ['', Validators.required],
      phone: ['', Validators.required],
      packageType: ['', Validators.required],
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
      } else {
        type = 1;
      }
      let user: User = {
        id: 0,
        email: this.registrationForm.value.email,
        password: this.registrationForm.value.password,
        firstname: this.registrationForm.value.firstName,
        lastname: this.registrationForm.value.lastName,
        companyName: this.registrationForm.value.companyName,
        taxId: this.registrationForm.value.taxId,
        address: this.registrationForm.value.address,
        city: this.registrationForm.value.city,
        country: this.registrationForm.value.country,
        phone: this.registrationForm.value.phone,
        packageType: parseInt(this.registrationForm.value.packageType),
        clientType: type,
        role: 0,
        isTwoFactorEnabled: this.registrationForm.value.twoFactorAuth,
      };
      this.userService.register(user).subscribe((response) => {
        if (response && response.isSuccess) {
          console.log('Registration request sent.');
          console.log(response);
          // Here we need to check what we got as a response:
          // 1. Successful registration without 2FA
          // 2. 2FA temp token and QR code

          if (!response.isTwoFactorEnabled) {
            console.log('Zahtjev za registraciju je poslat.');
            alert('Zahtjev za registraciju je poslat.');
            return;
          }

          // Activate modal and insert an image and wait for the user to enter the code.
          this.verify2faRequest.email = user.email;
          this.verify2faRequest.tempToken = undefined;
          const setup2faModal = document.getElementById('setup2faModal');
          if (setup2faModal) {
            this.qrCode = response.twoFactorQrCode;
            const modal = new bootstrap.Modal(setup2faModal);
            modal.show();
          }
        }
      });
    }
  }

  registerVerify2fa(): void {
    // TODO: send code
    // TODO: if code is correct, close modal and redirect
    // TODO: if not, remove code and show error
    // ?: maybe add cancel button and if pressed, send request to disable 2fa.

    this.userService.registerVerify2fa(this.verify2faRequest).subscribe(
      (response) => {
        if (response) {
          console.log('2FA verified.');
          alert('2FA verified. You can now login.');
          const setup2faModal = document.getElementById('setup2faModal');
          if (setup2faModal) {
            const modal = bootstrap.Modal.getInstance(setup2faModal);
            modal.hide();
          }
          this.router.navigate(['/login']);
        } else {
          console.error('2FA verification failed.');
          alert('2FA verification failed.');
        }
      },
      (error) => {
        console.error('2FA verification failed:', error);
        alert('2FA verification failed.');
      }
    );
  }
}
