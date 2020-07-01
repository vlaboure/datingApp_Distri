import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/Http';
import {BehaviorSubject, Subject } from 'rxjs';
import {map} from 'rxjs/operators';
import {JwtHelperService} from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../_models/User';
import { templateJitUrl } from '@angular/compiler';


/**************************************************************************************************/
              /****service injecté pour le login */
@Injectable({
  providedIn: 'root'
})


export class AuthService {
  baseUrl  = environment.apiUrl + 'auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  private _login;
  logOk = new Subject<boolean>();
    // photo par defaut peut être changée et est initalisée au démarrage
  photoUrl = new BehaviorSubject<string>('../../assets/user.png');
  currentPhotoUrl = this.photoUrl.asObservable();
// tslint:disable-next-line: typedef-whitespace
constructor(private Http : HttpClient) { }

changeMemberPhoto(photoUrl: string){
  // next propriété des promises change la photo de l'objet par la photo en argument
  this.photoUrl.next(photoUrl);
}
// tslint:disable-next-line: whitespace
// tslint:disable-next-line: typedef-whitespace

// ************************************************************** */
// le login avec le this.model passé en paramètre
  login(model: any){
                        // pipe empile chk fonction avec le resultat de la fonction précédente
    return this.Http.post(this.baseUrl + 'login', model).pipe(
                        // on passe user venant du serveur en paramètre
      map((response: any) => {
        const user = response;
        if (user){
                  // le token est enregistré en local pour les connexions futures
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(user.user));
          // user ---> token dans le http envoyé par le serveur
          this.currentUser = user.user;
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.changeMemberPhoto(this.currentUser.photoUrl);
        }
      })
    // tslint:disable-next-line: semicolon
    )
  }

  register(user: User){
    // requête post à l'api
    return this.Http.post(this.baseUrl + 'register', user);
  }

  //
  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }

  
  setLogin(model: any){
    this._login = model.userName;
    // tslint:disable-next-line: no-debugger
   // debugger;
    console.log(model.userName);

  }
  getLogin(){
    let tp = this._login;
   // this.clearLogin();
    return tp;
  }
  clearLogin(){
    this._login = undefined;
  }
  reset(user: User){
    // tslint:disable-next-line: no-debugger
    //debugger;
    return this.Http.put(this.baseUrl + 'resetUser', user);
  }
}
