import { Injectable, Query } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { BehaviorSubject, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  private hubConnection!: signalR.HubConnection;
  public onlineUsers$ = new BehaviorSubject<any[]>([]);
  public newMessage$ = new BehaviorSubject<any>({});
  public notifySenderAboutSeenMsg$ = new BehaviorSubject<any>({});
  constructor() 
  {
  
  }
  StartConection(){
    let loggedInUserInfo = sessionStorage.getItem("loggedInUser");
    let user = null;
    let token = '';
    if(loggedInUserInfo!=null)
    {
      let userInfo = JSON.parse(loggedInUserInfo);
      user = userInfo?.data?.user;
      token = userInfo?.data?.token;
    }

    //create hub connection
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:44366/Chat',{
        accessTokenFactory: () => token
      })
      .build();
    this.hubConnection.start()
      .then(()=> {
        console.log("connection started"); 
        this.StartListening()
      })
      .catch(err => console.error(err.toString()));  
  }
  StartListening(){
    this.hubConnection.on('OnlineUsers', (users: any[]) => {
      this.onlineUsers$.next(users)
    });
    this.hubConnection.on('ReceiveMessage', (message: any) => {
      this.newMessage$.next(message)
    });
    this.hubConnection.on("MessageSeen",(obj: any)=>{
      this.notifySenderAboutSeenMsg$.next(obj)
    });
  }

  SendMessage(payload:any){
    this.hubConnection.invoke("SendMessage",payload)
    .then(()=>{

    })
    .catch((err)=>{
      console.log(err)
    })
  }
  NotifySenderAboutMessageSeen(senderUserId: any, recieverUserId: any){
    this.hubConnection.invoke("NotifySenderAboutMessageSeen",senderUserId,recieverUserId)
    .then(()=>{

    })
    .catch((err)=>{
      console.log(err)
    })
  }
  
}
