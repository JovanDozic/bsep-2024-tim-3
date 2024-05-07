import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from 'src/features/layout/home/home.component';
import { AdminProfileComponent } from 'src/features/user/admin-profile/admin-profile.component';
import { ClientProfileComponent } from 'src/features/user/client-profile/client-profile.component';
import { EmployeeGuard } from 'src/features/user/employee-guard';
import { EmployeeProfileComponent } from 'src/features/user/employee-profile/employee-profile.component';
import { LoginComponent } from 'src/features/user/login/login.component';
import { MyProfileComponent } from 'src/features/user/my-profile/my-profile.component';
import { RegisterComponent } from 'src/features/user/register/register.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'home', component: HomeComponent },
  { path: 'my-profile', component: MyProfileComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'employee-profile', component:EmployeeProfileComponent, canActivate:[EmployeeGuard]},
  { path: 'admin-profile', component:AdminProfileComponent},
  { path: 'client-profile', component:ClientProfileComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
