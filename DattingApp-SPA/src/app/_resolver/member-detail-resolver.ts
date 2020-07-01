import {Injectable} from '@angular/core';
import {User} from '../_models/user';
import { UserService } from '../_services/user.service';
import {Resolve, Router, ActivatedRouteSnapshot} from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

// resolver pour passer les datas avant l'affichage des données
// injecté par le compilateur au lancement
@Injectable()
export class MemberDetailResolver implements Resolve<User>{
   constructor(private userService: UserService, private router: Router,
               private alertify: AlertifyService){}
   resolve(route: ActivatedRouteSnapshot): Observable<User>{
      return this.userService.getUser(route.params[`id`]).pipe(
         // catchError-> methode de rxjs
         catchError(error => {
            this.alertify.error('Erreur lors de la récupération des datas');
            this.router.navigate(['/members']);
            // of -> observable de rxjs
            return of(null); // return of--> return observable of(null)
         })
      );
   }
}