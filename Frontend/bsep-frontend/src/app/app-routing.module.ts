import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from 'src/features/layout/home/home.component';
import { AdminProfileComponent } from 'src/features/user/admin-profile/admin-profile.component';
import { AdminGuard } from 'src/features/user/guards/admin-guard';
import { ClientGuard } from 'src/features/user/guards/client-guard';
import { ClientProfileComponent } from 'src/features/user/client-profile/client-profile.component';
import { EmployeeGuard } from 'src/features/user/guards/employee-guard';
import { EmployeeProfileComponent } from 'src/features/user/employee-profile/employee-profile.component';
import { LoginComponent } from 'src/features/user/login/login.component';
import { RegisterComponent } from 'src/features/user/register/register.component';
import { RoleManagementComponent } from 'src/features/user/role-management/role-management.component';
import { PermissionManagementComponent } from 'src/features/user/permission-management/permission-management.component';
import { LoginPasswordlessComponent } from 'src/features/user/login-passwordless/login-passwordless.component';
import { LoginPasswordlessAuthenticateComponent } from 'src/features/user/login-passwordless-authenticate/login-passwordless-authenticate.component';
import { RegistrationRequestsManagementComponent } from 'src/features/user/registration-requests-management/registration-requests-management.component';
import { EmailVerificationComponent } from 'src/features/user/email-verification/email-verification.component';
import { ResetPasswordComponent } from 'src/features/user/reset-password/reset-password.component';
import { LogsComponent } from 'src/features/notifications/notifications_logs/logs.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'login-passwordless', component: LoginPasswordlessComponent },
  {
    path: 'authenticate-registration-request',
    component: EmailVerificationComponent,
  },
  {
    path: 'authenticate-passwordless',
    component: LoginPasswordlessAuthenticateComponent,
  },
  { path: 'register', component: RegisterComponent },
  { path: 'home', component: HomeComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  {
    path: 'employee-profile',
    component: EmployeeProfileComponent,
    canActivate: [EmployeeGuard],
  },
  {
    path: 'admin-profile',
    component: AdminProfileComponent,
    canActivate: [AdminGuard],
  },
  {
    path: 'role-management/:userId',
    component: RoleManagementComponent,
    canActivate: [AdminGuard],
  },
  {
    path: 'permission-management/:permissionId',
    component: PermissionManagementComponent,
    canActivate: [AdminGuard],
  },
  {
    path: 'client-profile',
    component: ClientProfileComponent,
  },
  {
    path: 'registration-requests',
    component: RegistrationRequestsManagementComponent,
    canActivate: [AdminGuard],
  },
  {
    path: 'reset-password',
    component: ResetPasswordComponent,
  },
  {
    path: 'logs',
    component: LogsComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
