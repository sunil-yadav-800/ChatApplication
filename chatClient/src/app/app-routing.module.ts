import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './Components/home/home.component';
import { LoginComponent } from './Components/login/login.component';
import { RegisterComponent } from './Components/register/register.component';

const routes: Routes = [
  {
    path:"",
    redirectTo :"home",
    pathMatch:"full"
  },
  {
    path:"home",
    component:HomeComponent
  },
  // {
  //   path:"login",
  //   component:LoginComponent
  // },
  {
    path:"register",
    component:RegisterComponent
  },
  {
    path:"**",
    redirectTo:'home'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
