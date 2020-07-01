import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/User';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() user: User;
  constructor(private userService : UserService, private autService : AuthService,
              private alertifyService : AlertifyService) { }

  ngOnInit() {
  }

  sendLike(id: number){
    this.userService.sendLike(this.autService.decodedToken.nameid,id).subscribe(
      data => {this.alertifyService.success("user liké : " + this.user.knownAs);},
      error =>{this.alertifyService.error("error au like "+ error)});
  }
}

