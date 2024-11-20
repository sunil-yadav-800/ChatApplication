import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonService } from 'src/app/Services/common.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  register: any = {name:'',email:'',password:''}
  constructor(private service:CommonService, private router:Router) { }

  ngOnInit(): void {
  }
  onRegister()
  {
    console.log(this.register);
    const payload = {
                      name:this.register.name,
                      email:this.register.email,
                      password:this.register.password
                    };
    this.register.name='';
    this.register.email='';
    this.register.password='';
    this.service.register(payload).subscribe((res:any)=>{
      let result = res;
      if(result.status)
      {
        alert("registered successfully");
        this.router.navigateByUrl("/home");
      }
      else
      {
        alert(result?.message);
      }
    },(err)=>{
      alert("something went wrong!");
    })
  }
}
