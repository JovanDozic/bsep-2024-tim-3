import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ClientProfileComponent } from './client-profile/client-profile.component';
import { AdminProfileComponent } from './admin-profile/admin-profile.component';
import { EmployeeProfileComponent } from './employee-profile/employee-profile.component';
import { RoleManagementComponent } from './role-management/role-management.component';
import { PermissionManagementComponent } from './permission-management/permission-management.component';
import { LoginPasswordlessComponent } from './login-passwordless/login-passwordless.component';
import { LoginPasswordlessAuthenticateComponent } from './login-passwordless-authenticate/login-passwordless-authenticate.component';

@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent,
    ClientProfileComponent,
    AdminProfileComponent,
    EmployeeProfileComponent,
    RoleManagementComponent,
    PermissionManagementComponent,
    LoginPasswordlessComponent,
    LoginPasswordlessAuthenticateComponent,
  ],
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
})
export class UserModule {}
