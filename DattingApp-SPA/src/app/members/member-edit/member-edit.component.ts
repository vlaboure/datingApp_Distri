import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/_models/User';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';


@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
  
})
export class MemberEditComponent implements OnInit {
  // variable pour accéder au dom et modifier le editForm pour faire disparaître le message d'avertissement
  // Vous avez fait des changements sans valider, vos modifications seront perdues
  @ViewChild('editForm', {static: true}) editForm: NgForm;
  user: User;
  photoUrl: string;
  // le navigateur et non l'appli envoie un message si quitter après modif sans enregistrer
  @HostListener('window:beforeunload',['$event'])
  unloadNotification($event: any){
    if (this.editForm.dirty){
      $event.returnValue = true;
    }
  }
  constructor(private route: ActivatedRoute, private alertify: AlertifyService, private userService: UserService,
                                             private authService: AuthService) { }

  ngOnInit() {
    this.route.data.subscribe(data =>{
      this.user = data['user'];
    });
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl= photoUrl);
  }
  updateUser(){
    this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(next => {
      this.alertify.success('mise à jour réussie');
      this.editForm.reset(this.user);
      this.editForm.reset(this.user);},error => {this.alertify.error(error); });
  }

  updateMainPhoto(photoUrl){
    this.user.photoUrl = photoUrl;
  }
}
