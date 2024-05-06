import { Component, OnInit } from '@angular/core';
import { UserService } from '../user.service';
import { Router } from '@angular/router';
import { ClientType, User } from '../model/user.model';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})

export class RegisterComponent implements OnInit {
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
  type: ClientType = ClientType.NaturalPerson;
  user: User[] = [];

  selectedOption: string = "natural";

  constructor(private userService: UserService, private router: Router) {} 

  ngOnInit(){
    /*this.userService.getUsers().subscribe(
      (result)=>{
        this.users = result;
      }
    )*/
  }

  validateForm() : boolean {
    if (this.password !== this.confirmPassword) {
      return false;
    } else {
      return true;
    }
  }

  onChange() : void {
    if(this.selectedOption == "natural") {
      this.isNatural = true
    } else {
      this.isNatural = false
    }
  }

  register() : void {
    if (this.validateForm()) {
      const user = {
        name: this.name,
        email: this.email,
        password: this.password,
        confirmPassword: this.confirmPassword,
        address: this.address,
        city: this.city,
        country: this.country,
        phoneNumber: this.phoneNumber,
        type: this.type,
      };

      /*for(let user of this.users){
        if(user.email === user.email){
          console.error('Email duplicate.');
          alert('The entered email is already used!');
          break;
        }
      }*/

      /*this.userService.saveUser(user, this.password).subscribe(
        response => {
            console.log('User saved successfully', response);
            this.router.navigate(['/home']);
        },
        error => {
            console.error('Error saving user', error);
            alert('There was an error while saving the data! Please try again.');
        }
    );*/
    this.router.navigate(['/home']);
    
    }
    else{
      alert('Passwords do not match');

    }
  }

  onConfirmPasswordInput(): void {
    const registerButton: HTMLButtonElement | null = document.getElementById('registerButton') as HTMLButtonElement | null;

    if (registerButton) {
      registerButton.disabled = this.password !== this.confirmPassword;
    }
  }
}
