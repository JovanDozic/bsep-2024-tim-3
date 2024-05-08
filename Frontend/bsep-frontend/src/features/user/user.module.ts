import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyProfileComponent } from './my-profile/my-profile.component';
import { ClientProfileComponent } from './client-profile/client-profile.component';
import { AdminProfileComponent } from './admin-profile/admin-profile.component';
import { EmployeeProfileComponent } from './employee-profile/employee-profile.component';



@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent,
    MyProfileComponent,
    ClientProfileComponent,
    AdminProfileComponent,
    EmployeeProfileComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule
  ]
})
export class UserModule { }
