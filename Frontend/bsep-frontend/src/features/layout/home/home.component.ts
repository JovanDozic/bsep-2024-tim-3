import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TokenStorage } from 'src/features/user/jwt/token.service';
import { User } from 'src/features/user/model/user.model';
import { UserService } from 'src/features/user/user.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  loggedUser: User;
  allUsers: User[];
  isAdmin: boolean = false;
  isClient: boolean = false;
  isEmployee: boolean = false;
  showPopup = false;
  //changePasswordForm: FormGroup;
  newPassword: string;

  constructor(private formBuilder: FormBuilder,private userService: UserService, private router: Router, private tokenStorage: TokenStorage){}

  ngOnInit() : void {
    this.newPassword = '';
    this.fetchUserWithDelay();
    
    this.userService.getAllUsers().subscribe(users => {
      this.allUsers = users;
      //this.allClients = this.allUsers.filter(user => user.role === 0);
      //this.allEmployees = this.allUsers.filter(user => user.role === 1);
    },
    error => {
      console.error('Greska kod dobavljanja svih:', error);
    });
/*
    this.changePasswordForm = this.formBuilder.group({
      password: ['', Validators.required]
    })*/
  }

  fetchUserWithDelay(): void {
    setTimeout(() => {
      this.loadUser();
    }, 2500); // Adjust the delay time as needed
  }

  loadUser() {
    this.userService.getUserById(this.tokenStorage.getUserId()).subscribe(
      (user: User) => {
        this.loggedUser = user;
        if(this.loggedUser.role === 1 && this.loggedUser.clientType === 0) {
          this.showPopup = true;
        }
        this.isLoggedIn();
      },
      error => {
        console.error('Error fetching user:', error);
      }
    )
  }
  openPopup(): void {
    this.showPopup = true;
  }


  isLoggedIn() : void {
    if(this.loggedUser.role == 2){
      this.isAdmin = true;
    } else if (this.loggedUser.role == 1){
      this.isEmployee == true;
    } else {
      this.isClient == true;
    }
  }

  roleManagement(userId: number) : void {
    this.router.navigate(['/role-management', userId]);
  }

  permissionManagement(permissionId: number) : void {
    this.router.navigate(['/permission-management', permissionId]);
  }
  updateUser() {
    const newPasswordInput = document.getElementById('newPasswordInput') as HTMLInputElement;
  const newPassword = newPasswordInput.value;
    this.loggedUser.password = newPassword;
    this.loggedUser.clientType = 1;
    this.userService.updateUser(this.loggedUser).subscribe(
        (updated: boolean) => {
            if (updated) {
              this.router.navigate(['/login']);
                console.log('Employee updated successfully');
              } else {
                console.error('Failed to update employee');
            }
        },
        error => {
            console.error('Error updating employee:', error);
        }
    );
    this.userService.logout();
    this.router.navigate(['/login']);
}

}
