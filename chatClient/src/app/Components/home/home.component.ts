import { Component, OnInit } from '@angular/core';
import { ChatService } from 'src/app/Services/chat.service';
import { CommonService } from 'src/app/Services/common.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  //allOriginalUsers: any[]=[]
  allUsers: any[] = [];
  onlineUsers!: any[]
  selectedUser!: any
  loginVisible: boolean = true;
  loggedInUser: any = null
  constructor(private service:CommonService, private chatService:ChatService) 
  { 
    this.chatService.onlineUsers$.subscribe((users)=>{
      this.onlineUsers = users.filter(user=>user.id != this.loggedInUser.userId);
      console.log(this.allUsers)
      this.allUsers.forEach((user)=>{
        if(user?.IsOnline)
        {
          user.IsOnline=false;
        }
      });
      this.onlineUsers.forEach((onlineUser) => {
        const matchedUser = this.allUsers.find(user => user?.Id === onlineUser?.id);
        if (matchedUser) {
          matchedUser.IsOnline = true;
        }
      });
      
      console.log(this.onlineUsers)
      console.log(this.allUsers);
    },(err)=>{
      console.log(err);
    })
  }

  ngOnInit(): void {
    let loggedInUserInfo = sessionStorage.getItem("loggedInUser");
    if(loggedInUserInfo!=null)
    {
      let userInfo = JSON.parse(loggedInUserInfo);
      this.loggedInUser = userInfo?.data.user;
      //hide login component
      this.onCrossClick();
    }
    this.loginAlert(true);
  }

  // getDate(){
  //   this.service.getData().subscribe((res)=>{
  //     console.log(res);
  //   },(err)=>{
  //     alert(err);
  //   });
  // }
  onUserChange(user:any)
  {
    this.selectedUser = user;
  }
  isUserLoggedIn(): boolean{
    return this.loggedInUser==null?false:true;
  }
  onLoginClick(){
    this.loginVisible=true;
  }
  onCrossClick(){
    this.loginVisible=false;
  }
  loginAlert(e:any){
    let loggedInUserInfo = sessionStorage.getItem("loggedInUser");
    if(loggedInUserInfo!=null)
    {
      let userInfo = JSON.parse(loggedInUserInfo);
      this.loggedInUser = userInfo?.data.user;
      //hide login component
      this.onCrossClick();
      this.getAllUsers();
    }
  }
  getAllUsers(){
    this.service.getAllUsers(this.loggedInUser.userId).subscribe((res:any)=>{
      if(res?.status)
      {
        //console.log(res.data)
        this.allUsers = res?.data?.filter((user:any)=>user?.Id != this.loggedInUser?.userId)
        console.log(this.allUsers);
        this.chatService.StartConection();
      }
    },(err)=>{
      alert(err)
    })
  }
  onSelectedUserReadMsgs(){
    let currUser = this.allUsers.find(u=> u.Id == this.selectedUser.Id);
    if(currUser)
    {
      currUser.UnreadMessages = 0
    }
  }
  IncrementUnreadMsgs(fromUserId: any) {
    let fromUser = this.allUsers.find(u=> u.Id == fromUserId);
    if(fromUser)
    {
      fromUser.UnreadMessages+=1;
    }
  }
  
}
