import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { CommonService } from 'src/app/Services/common.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  email:any=''
  password:any=''
  @Output() loginAlert = new EventEmitter<any>();
  constructor(private service: CommonService) { }

  ngOnInit(): void {
  }
  onLogin()
  {
    this.service.displayLoader(true)
    const payload = {email:this.email,password:this.password}
    this.email=''
    this.password=''
    this.service.login(payload).subscribe((res:any)=>{
      console.log(res);
      this.service.displayLoader(false)
      if(res?.status)
      {
        alert("logged in");
        sessionStorage.setItem('loggedInUser',JSON.stringify(res));
        this.loginAlert.emit(true);
      }
      else
      {
        alert("error");
      }
    },(err)=>{
      this.service.displayLoader(false)
      alert("err: error");
    })
  }

}
