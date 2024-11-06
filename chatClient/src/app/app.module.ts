import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './Components/home/home.component';
import { UserListComponent } from './Components/user-list/user-list.component';
import { UserChatComponent } from './Components/user-chat/user-chat.component';
import { CommonService } from './Services/common.service';
import {HttpClientModule} from '@angular/common/http';
import { LoginComponent } from './Components/login/login.component';
import { RegisterComponent } from './Components/register/register.component';
import { FormsModule } from '@angular/forms';
import {MaterialAngularModule} from './material-angular.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'
import { ChatService } from './Services/chat.service';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    UserListComponent,
    UserChatComponent,
    LoginComponent,
    RegisterComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    MaterialAngularModule,
    BrowserAnimationsModule
  ],
  providers: [CommonService,ChatService],
  bootstrap: [AppComponent]
})
export class AppModule { }
