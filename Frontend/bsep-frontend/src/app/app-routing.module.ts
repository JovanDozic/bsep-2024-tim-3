import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from 'src/features/layout/home/home.component';
import { LoginComponent } from 'src/features/user/login/login.component';
import { MyProfileComponent } from 'src/features/user/my-profile/my-profile.component';
import { RegisterComponent } from 'src/features/user/register/register.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'home', component: HomeComponent },
  { path: 'my-profile', component: MyProfileComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
