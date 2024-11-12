import { Component, OnInit, Input, ViewChild, ElementRef, AfterViewChecked, OnChanges, SimpleChanges, Output, EventEmitter } from '@angular/core';
import { ChatService } from 'src/app/Services/chat.service';
import { CommonService } from 'src/app/Services/common.service';

@Component({
  selector: 'app-user-chat',
  templateUrl: './user-chat.component.html',
  styleUrls: ['./user-chat.component.css']
})
export class UserChatComponent implements OnInit, OnChanges, AfterViewChecked {

  @Input() user:any
  @Input() loggedInUser: any
  @ViewChild('scrollMe') private myScrollContainer!: ElementRef;
  allMessages: any[]=[]
  msg: string=''
  @Output() onSelectedUserReadMsgs = new EventEmitter<any>();
  @Output() IncrementUnreadMsgs = new EventEmitter<any>();
  isScrollToBottom: boolean = true;
  constructor(private service:CommonService, private chatService:ChatService)
  {
    this.chatService.newMessage$.subscribe((message)=>{
      if((message?.from == this.loggedInUser?.userId && message?.to == this.user?.Id) || (message?.to == this.loggedInUser?.userId && message?.from == this.user?.Id))
      {
        let obj = {
          Id: message?.id,
          Content: message?.content,
          From : message?.from,
          To : message?.to,
          CreatedAt : message?.createdAt,
          IsSeen : false
        }
        this.allMessages.push(obj)
        
        this.playSound()

        if(message?.to == this.loggedInUser?.userId && message?.from == this.user?.Id){
          setTimeout(()=>
          {
            this.markUnreadMessageAsSeen();
          },1000)
        }
      }
      else if(message?.to == this.loggedInUser?.userId)
      {
        this.IncrementUnreadMsgs.emit(message.from)
      }
    })

    this.chatService.notifySenderAboutSeenMsg$.subscribe((obj)=>{
      
      if(obj.senderUserId == this.loggedInUser?.userId && obj.recieverUserId == this.user?.Id)
      {
        const unreadMessages = this.allMessages.filter(msg => 
          msg.From == this.loggedInUser?.userId && 
          msg.To == this.user?.Id &&
           msg.IsSeen == false
           );

           unreadMessages.forEach(um=>{
            let m = this.allMessages.find(msg=> msg.Id == um.Id);
            if(m)
            {
              m.IsSeen = true;
            }
           });
      }
    })
  }
 
  ngOnChanges(changes: SimpleChanges) {
    // Check if the user input has changed
    if (changes['user'] && changes['user'].currentValue !== changes['user'].previousValue) {
      //get user chat from db.
      this.getAllChat();
    }
  }
  //TODO: need to unsbscribe this ondestroy
  ngAfterViewChecked() {
    this.scrollToBottom();
  }
  ngOnInit(): void {
    
  }

  scrollToBottom(): void {
    try {
      if(this.myScrollContainer)
      {
        this.myScrollContainer.nativeElement.scrollIntoView({ behavior: 'smooth' });
      }
    } catch (err) {
      console.log(err);
    }
  }

  getAllChat(){
    if(this.user && this.loggedInUser)
    {
      this.service.getAllMessagesByIds(this.loggedInUser.userId,this.user.Id).subscribe((res:any)=>{
        if(res.status)
        {
          this.allMessages = res.data;
          this.markUnreadMessageAsSeen();
        }
      },(err)=>{
        console.log(err);
      })
    }
  }

 async SendMessage(){
    let payload = {
      Content: this.msg,
      From: this.loggedInUser.userId,
      To : this.user.Id
    }
    if(this.msg.trim().length>0)
    {
      this.chatService.SendMessage(payload)
      this.msg=''
    }
  }
  async playSound(){
    // let audio = new Audio();
    // audio.src='../assets/chat-music.wav';
    // await audio.play();
  }
  async markUnreadMessageAsSeen(){
    const unreadMessages = this.allMessages.filter(msg => 
      msg.From == this.user?.Id && 
      msg.To == this.loggedInUser?.userId &&
       msg.IsSeen == false
       )

    const unreadMessageIds = unreadMessages.map(msg => { 
      return {Id: msg.Id}
    });
    if(unreadMessages && unreadMessages.length > 0)
    {
      this.service.MarkMssagesAsSeen(unreadMessageIds).subscribe((res: any)=>{
        if(res?.status)
        {
          unreadMessageIds.forEach(um=>{
            let m =this.allMessages.find(msg=> msg.Id == um.Id);
            if(m)
            {
              m.IsSeen = true;
              
            }
            console.log(res)
          });
          //remove badge showing no. of unread msg
          this.onSelectedUserReadMsgs.emit();

          //notify sender about msg read
          this.chatService.NotifySenderAboutMessageSeen(this.user.Id, this.loggedInUser.userId)
        }
      },(err)=>{
        console.log(err);
      })
    }
  }

}
