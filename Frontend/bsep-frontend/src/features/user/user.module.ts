import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MyProfileComponent } from './my-profile/my-profile.component';



@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent,
    MyProfileComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule
  ]
})
export class UserModule { }
