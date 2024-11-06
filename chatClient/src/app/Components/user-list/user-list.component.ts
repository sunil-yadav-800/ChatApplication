import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit {

  @Input() users!: any[]
  @Input() allUsers: any[] = []
  @Output() onUserChange = new EventEmitter<any>();
  constructor() { }

  ngOnInit(): void {
  }
  onUserSelect(user:any){
    //alert("Selected User: "+user);
    this.onUserChange.emit(user);
  }

}
