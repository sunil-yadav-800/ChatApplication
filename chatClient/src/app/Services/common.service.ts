import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';

@Injectable({
  providedIn: 'root'
})
export class CommonService {

  BaseUrl:string = "https://localhost:44366/"
  public loaderStatus: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  constructor(private http: HttpClient) { }

  login(payload:any){
    return this.http.post(this.BaseUrl+"api/Auth/Login",payload);
  }
  register(payload:any){
    return this.http.post(this.BaseUrl+"api/Auth/Register",payload);
  }
  getAllMessagesByIds(loggedInUser:any, otherUser:any)
  {
    console.log(loggedInUser+" - "+ otherUser)
    return this.http.get(`${this.BaseUrl}api/Message/GetMessagesByIds/${loggedInUser}/${otherUser}`);
  }
  getAllUsers(userId:any){
    return this.http.get(this.BaseUrl+"api/Auth/GetAllUsers/"+userId);
  }
  displayLoader(value:boolean){
    this.loaderStatus.next(value);
  }
  MarkMssagesAsSeen(payload: any){
    return this.http.post(this.BaseUrl+"api/Message/MarkMssagesAsSeen",payload);
  }
}
