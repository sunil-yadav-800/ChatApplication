import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonService } from 'src/app/Services/common.service';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit {

  @Input() users!: any[]
  @Input() allUsers: any[] = []
  @Output() onUserChange = new EventEmitter<any>();
  @Output() onSelectUser = new EventEmitter<any>();
  searchTerm: string = ''
  private debounceTimer: any;
  searchUserList: any[] = []
  constructor(private service:CommonService) { }

  ngOnInit(): void {
  }
  onUserSelect(user:any){
    //alert("Selected User: "+user);
    this.onUserChange.emit(user);
  }
  onChange(event:any){
    //console.log(this.searchTerm)
    this.debounce(()=>{
      //console.log(this.searchTerm);
      this.searchUsers();
    });
  }
  debounce(callback:any){
    if(this.debounceTimer)
    {
      clearInterval(this.debounceTimer);
    }
    this.debounceTimer =  setTimeout(()=>{
        callback();
      },500);
  }
  searchUsers(){
    if(this.searchTerm?.trim().length>0)
    {
      this.service.SearchUsers(this.searchTerm.trim()).subscribe((res:any)=>{
        this.searchUserList = res?.data
      },(err)=>{

      });
    }
  }
  onClose(event:any)
  {
    this.searchUserList=[];
    //console.log(event);
  }
  onOptionSelect(event:any)
  {
    //console.log(event?.option?.value);
    let selectedUser = this.allUsers.find(u=> u.Id == event?.option?.value?.Id?.toLowerCase());
    if(selectedUser === undefined){
      this.onSelectUser.emit(event?.option?.value)
    }
    else{
      this.onUserSelect(selectedUser);
    }
    this.searchTerm=''
  }
}
